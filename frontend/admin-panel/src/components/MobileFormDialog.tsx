import { useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import { useAdminMobile, useCreateMobile, useUpdateMobile } from '../hooks/useMobiles';
import ImageManager from './ImageManager';
import type { MobileRequest } from '../types/api';

const schema = z.object({
  brand: z.string().min(1, 'Brand is required').max(100),
  model: z.string().min(1, 'Model is required').max(150),
  storage: z.string().max(50).optional(),
  color: z.string().max(50).optional(),
  conditionGrade: z.enum(['New', 'LikeNew', 'Good', 'Fair']),
  price: z.coerce.number().positive('Price must be greater than 0'),
  description: z.string().optional(),
  status: z.enum(['Active', 'SoldOut', 'Draft']),
});

type FormInput = z.input<typeof schema>;
type FormValues = z.output<typeof schema>;

const DEFAULTS: FormInput = {
  brand: '',
  model: '',
  storage: '',
  color: '',
  conditionGrade: 'Good',
  price: 0,
  description: '',
  status: 'Draft',
};

interface MobileFormDialogProps {
  open: boolean;
  mobileId: number | null;
  onClose: () => void;
}

export default function MobileFormDialog({ open, mobileId, onClose }: MobileFormDialogProps) {
  const isEditing = mobileId !== null;
  const { data: existing } = useAdminMobile(mobileId);
  const createMutation = useCreateMobile();
  const updateMutation = useUpdateMobile();

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
        existing
          ? {
              brand: existing.brand,
              model: existing.model,
              storage: existing.storage ?? '',
              color: existing.color ?? '',
              conditionGrade: existing.conditionGrade,
              price: existing.price,
              description: existing.description ?? '',
              status: existing.status,
            }
          : DEFAULTS,
      );
    }
  }, [open, existing, reset]);

  const mutation = isEditing ? updateMutation : createMutation;

  const onSubmit = (values: FormValues) => {
    const request: MobileRequest = { ...values };
    if (isEditing) {
      updateMutation.mutate({ id: mobileId, request }, { onSuccess: onClose });
    } else {
      createMutation.mutate(request, { onSuccess: onClose });
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEditing ? 'Edit Mobile' : 'Add Mobile'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <Stack direction="row" spacing={2}>
            <TextField label="Brand" fullWidth {...register('brand')} error={!!errors.brand} helperText={errors.brand?.message}
            slotProps={{ inputLabel: { shrink: true } }} />
            <TextField label="Model" fullWidth {...register('model')} error={!!errors.model} helperText={errors.model?.message}
            slotProps={{ inputLabel: { shrink: true } }} />
          </Stack>
          <Stack direction="row" spacing={2}>
            <TextField label="Storage (e.g. 128GB)" fullWidth {...register('storage')}
            slotProps={{ inputLabel: { shrink: true } }} />
            <TextField label="Color" fullWidth {...register('color')}
            slotProps={{ inputLabel: { shrink: true } }} />
          </Stack>
          <Stack direction="row" spacing={2}>
            <Controller
              name="conditionGrade"
              control={control}
              render={({ field }) => (
                <TextField select label="Condition" fullWidth {...field}>
                  {['New', 'LikeNew', 'Good', 'Fair'].map((c) => (
                    <MenuItem key={c} value={c}>
                      {c}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <TextField
              label="Price"
              type="number"
              fullWidth
              {...register('price')}
              error={!!errors.price}
              helperText={errors.price?.message}
            slotProps={{ inputLabel: { shrink: true } }}
            />
          </Stack>
          <Controller
            name="status"
            control={control}
            render={({ field }) => (
              <TextField select label="Status" fullWidth {...field}>
                {['Draft', 'Active', 'SoldOut'].map((s) => (
                  <MenuItem key={s} value={s}>
                    {s}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />
          <TextField label="Description" fullWidth multiline minRows={2} {...register('description')}
            slotProps={{ inputLabel: { shrink: true } }} />

          {mutation.isError && <Alert severity="error">Failed to save. Check the fields and try again.</Alert>}

          {isEditing && existing && (
            <>
              <Divider />
              <ImageManager
                ownerType="mobiles"
                ownerId={mobileId}
                imageIds={existing.imageIds}
                primaryImageId={existing.imageIds[0] ?? null}
                invalidateKey={['admin-mobile', mobileId]}
              />
            </>
          )}
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
