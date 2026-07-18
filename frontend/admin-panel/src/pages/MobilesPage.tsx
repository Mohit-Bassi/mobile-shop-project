import { useState } from 'react';
import { Box, Button, Chip, IconButton, MenuItem, Stack, TextField, Tooltip, Typography } from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useAdminMobiles, useDeleteMobile, useUpdateMobileStatus } from '../hooks/useMobiles';
import MobileFormDialog from '../components/MobileFormDialog';
import type { MobileListItem } from '../types/api';

const STATUS_COLORS: Record<string, 'success' | 'warning' | 'default'> = {
  Active: 'success',
  SoldOut: 'warning',
  Draft: 'default',
};

export default function MobilesPage() {
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(0);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);

  const { data, isLoading } = useAdminMobiles({ status: statusFilter || undefined, page: page + 1, pageSize: 10 });
  const updateStatusMutation = useUpdateMobileStatus();
  const deleteMutation = useDeleteMobile();

  const openCreate = () => {
    setEditingId(null);
    setDialogOpen(true);
  };

  const openEdit = (id: number) => {
    setEditingId(id);
    setDialogOpen(true);
  };

  const columns: GridColDef<MobileListItem>[] = [
    { field: 'brand', headerName: 'Brand', flex: 1 },
    { field: 'model', headerName: 'Model', flex: 1.2 },
    { field: 'conditionGrade', headerName: 'Condition', flex: 0.8 },
    {
      field: 'price',
      headerName: 'Price',
      flex: 0.8,
      renderCell: (params) => `₹${params.row.price.toLocaleString()}`,
    },
    {
      field: 'status',
      headerName: 'Status',
      flex: 0.8,
      renderCell: (params) => <Chip label={params.row.status} size="small" color={STATUS_COLORS[params.row.status]} />,
    },
    {
      field: 'actions',
      headerName: 'Actions',
      flex: 1.5,
      sortable: false,
      renderCell: (params) => (
        <Stack direction="row" spacing={0.5}>
          <Tooltip title="Edit">
            <IconButton size="small" onClick={() => openEdit(params.row.mobileId)}>
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <TextField
            select
            size="small"
            variant="standard"
            value={params.row.status}
            onChange={(e) => updateStatusMutation.mutate({ id: params.row.mobileId, status: e.target.value })}
            sx={{ minWidth: 90 }}
          >
            {['Draft', 'Active', 'SoldOut'].map((s) => (
              <MenuItem key={s} value={s}>
                {s}
              </MenuItem>
            ))}
          </TextField>
          <Tooltip title="Delete (hide from public site)">
            <IconButton size="small" onClick={() => deleteMutation.mutate(params.row.mobileId)}>
              <DeleteIcon fontSize="small" color="error" />
            </IconButton>
          </Tooltip>
        </Stack>
      ),
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>
          Mobiles
        </Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          Add Mobile
        </Button>
      </Stack>

      <TextField
        select
        label="Status"
        size="small"
        value={statusFilter}
        onChange={(e) => setStatusFilter(e.target.value)}
        sx={{ mb: 2, minWidth: 160 }}
      >
        <MenuItem value="">All</MenuItem>
        {['Draft', 'Active', 'SoldOut'].map((s) => (
          <MenuItem key={s} value={s}>
            {s}
          </MenuItem>
        ))}
      </TextField>

      <Box sx={{ height: 520 }}>
        <DataGrid
          rows={data?.items ?? []}
          columns={columns}
          getRowId={(row) => row.mobileId}
          loading={isLoading}
          paginationMode="server"
          rowCount={data?.totalCount ?? 0}
          paginationModel={{ page, pageSize: 10 }}
          onPaginationModelChange={(model) => setPage(model.page)}
          pageSizeOptions={[10]}
          disableRowSelectionOnClick
        />
      </Box>

      <MobileFormDialog open={dialogOpen} mobileId={editingId} onClose={() => setDialogOpen(false)} />
    </Box>
  );
}
