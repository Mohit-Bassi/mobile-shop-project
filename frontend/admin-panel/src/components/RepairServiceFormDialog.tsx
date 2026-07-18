import { useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Alert, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, Stack, Switch, TextField } from '@mui/material';
import { useCreateRepairService, useUpdateRepairService } from '../hooks/useRepairServices';
import type { RepairService, RepairServiceRequest } from '../types/api';

const schema = z.object({
  title: z.string().min(1, 'Title is required').max(150),
  description: z.string().optional(),
  priceFrom: z.union([z.coerce.number().positive(), z.literal('')]).optional(),
  estimatedTurnaround: z.string().max(100).optional(),
  isActive: z.boolean(),
  displayOrder: z.coerce.number().int(),
});

type FormInput = z.input<typeof schema>;
type FormValues = z.output<typeof schema>;

const DEFAULTS: FormInput = { title: '', description: '', priceFrom: '', estimatedTurnaround: '', isActive: true, displayOrder: 0 };

interface RepairServiceFormDialogProps {
  open: boolean;
  repairService: RepairService | null;
  onClose: () => void;
}

export default function RepairServiceFormDialog({ open, repairService, onClose }: RepairServiceFormDialogProps) {
  const isEditing = repairService !== null;
  const createMutation = useCreateRepairService();
  const updateMutation = useUpdateRepairService();

  const {
    register,
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormInput, unknown, FormValues>({ resolver: zodResolver(schema), defaultValues: DEFAULTS });

  useEffect(() => {
    if (open) {
      reset(
        repairService
          ? {
              title: repairService.title,
              description: repairService.description ?? '',
              priceFrom: repairService.priceFrom ?? '',
              estimatedTurnaround: repairService.estimatedTurnaround ?? '',
              isActive: true,
              displayOrder: repairService.displayOrder,
            }
          : DEFAULTS,
      );
    }
  }, [open, repairService, reset]);

  const mutation = isEditing ? updateMutation : createMutation;

  const onSubmit = (values: FormValues) => {
    const request: RepairServiceRequest = {
      ...values,
      priceFrom: values.priceFrom === '' ? undefined : Number(values.priceFrom),
    };
    if (isEditing) {
      updateMutation.mutate({ id: repairService.repairServiceId, request }, { onSuccess: onClose });
    } else {
      createMutation.mutate(request, { onSuccess: onClose });
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{isEditing ? 'Edit Repair Service' : 'Add Repair Service'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField label="Title" fullWidth {...register('title')} error={!!errors.title} helperText={errors.title?.message}
            slotProps={{ inputLabel: { shrink: true } }} />
          <TextField label="Description" fullWidth multiline minRows={2} {...register('description')}
            slotProps={{ inputLabel: { shrink: true } }} />
          <TextField label="Price from (leave blank for 'contact for quote')" type="number" fullWidth {...register('priceFrom')}
            slotProps={{ inputLabel: { shrink: true } }} />
          <TextField label="Estimated turnaround" fullWidth {...register('estimatedTurnaround')}
            slotProps={{ inputLabel: { shrink: true } }} />
          <TextField label="Display order" type="number" fullWidth {...register('displayOrder')}
            slotProps={{ inputLabel: { shrink: true } }} />
          <Controller
            name="isActive"
            control={control}
            render={({ field }) => (
              <FormControlLabel control={<Switch checked={field.value} onChange={(e) => field.onChange(e.target.checked)} />} label="Active" />
            )}
          />
          {mutation.isError && <Alert severity="error">Failed to save. Check the fields and try again.</Alert>}
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit(onSubmit)} disabled={mutation.isPending}>
          {mutation.isPending ? 'Saving…' : 'Save'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
