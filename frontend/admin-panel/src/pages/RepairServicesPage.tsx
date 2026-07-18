import { useState } from 'react';
import {
  Box,
  Button,
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useDeleteRepairService, useRepairServices } from '../hooks/useRepairServices';
import RepairServiceFormDialog from '../components/RepairServiceFormDialog';
import type { RepairService } from '../types/api';

export default function RepairServicesPage() {
  const { data: services, isLoading } = useRepairServices();
  const deleteMutation = useDeleteRepairService();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<RepairService | null>(null);

  const openCreate = () => {
    setEditing(null);
    setDialogOpen(true);
  };

  const openEdit = (service: RepairService) => {
    setEditing(service);
    setDialogOpen(true);
  };

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>
          Repair Services
        </Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          Add Repair Service
        </Button>
      </Stack>

      <TableContainer component={Paper} variant="outlined">
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Title</TableCell>
              <TableCell>Price from</TableCell>
              <TableCell>Turnaround</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {!isLoading &&
              services?.map((service) => (
                <TableRow key={service.repairServiceId}>
                  <TableCell>{service.title}</TableCell>
                  <TableCell>{service.priceFrom ? `₹${service.priceFrom.toLocaleString()}` : 'Contact for quote'}</TableCell>
                  <TableCell>{service.estimatedTurnaround ?? '—'}</TableCell>
                  <TableCell align="right">
                    <Tooltip title="Edit">
                      <IconButton size="small" onClick={() => openEdit(service)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Deactivate">
                      <IconButton size="small" onClick={() => deleteMutation.mutate(service.repairServiceId)}>
                        <DeleteIcon fontSize="small" color="error" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
          </TableBody>
        </Table>
      </TableContainer>

      <RepairServiceFormDialog open={dialogOpen} repairService={editing} onClose={() => setDialogOpen(false)} />
    </Box>
  );
}
