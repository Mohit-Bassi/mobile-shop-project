import { useState } from 'react';
import { Box, Chip, MenuItem, TextField, Typography } from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useInquiries, useUpdateInquiryStatus } from '../hooks/useInquiries';
import type { Inquiry } from '../types/api';

const STATUS_COLORS: Record<string, 'info' | 'warning' | 'success'> = {
  New: 'info',
  Contacted: 'warning',
  Closed: 'success',
};

export default function InquiriesPage() {
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(0);

  const { data, isLoading } = useInquiries({ status: statusFilter || undefined, page: page + 1, pageSize: 10 });
  const updateStatusMutation = useUpdateInquiryStatus();

  const columns: GridColDef<Inquiry>[] = [
    {
      field: 'createdAtUtc',
      headerName: 'Received',
      flex: 1,
      renderCell: (params) => new Date(params.row.createdAtUtc).toLocaleString(),
    },
    { field: 'listingType', headerName: 'About', flex: 0.8 },
    { field: 'customerName', headerName: 'Name', flex: 1 },
    { field: 'customerPhone', headerName: 'Phone', flex: 1 },
    { field: 'message', headerName: 'Message', flex: 1.5 },
    {
      field: 'status',
      headerName: 'Status',
      flex: 1,
      renderCell: (params) => (
        <TextField
          select
          size="small"
          variant="standard"
          value={params.row.status}
          onChange={(e) => updateStatusMutation.mutate({ id: params.row.inquiryId, status: e.target.value })}
          sx={{ minWidth: 110 }}
        >
          {['New', 'Contacted', 'Closed'].map((s) => (
            <MenuItem key={s} value={s}>
              <Chip label={s} size="small" color={STATUS_COLORS[s]} />
            </MenuItem>
          ))}
        </TextField>
      ),
    },
  ];

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 700, mb: 2 }}>
        Inquiries
      </Typography>

      <TextField
        select
        label="Status"
        size="small"
        value={statusFilter}
        onChange={(e) => setStatusFilter(e.target.value)}
        sx={{ mb: 2, minWidth: 160 }}
      >
        <MenuItem value="">All</MenuItem>
        {['New', 'Contacted', 'Closed'].map((s) => (
          <MenuItem key={s} value={s}>
            {s}
          </MenuItem>
        ))}
      </TextField>

      <Box sx={{ height: 560 }}>
        <DataGrid
          rows={data?.items ?? []}
          columns={columns}
          getRowId={(row) => row.inquiryId}
          loading={isLoading}
          paginationMode="server"
          rowCount={data?.totalCount ?? 0}
          paginationModel={{ page, pageSize: 10 }}
          onPaginationModelChange={(model) => setPage(model.page)}
          pageSizeOptions={[10]}
          disableRowSelectionOnClick
        />
      </Box>
    </Box>
  );
}
