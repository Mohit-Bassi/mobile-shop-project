import { useEffect, useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  Alert,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useAdminAccessory, useCreateAccessory, useUpdateAccessory } from '../hooks/useAccessories';
import { useCategories } from '../hooks/useCategories';
import ImageManager from './ImageManager';
import type { AccessoryRequest, CompatibleMobile } from '../types/api';

const schema = z.object({
  name: z.string().min(1, 'Name is required').max(150),
  categoryId: z.coerce.number().positive('Select a category'),
  price: z.coerce.number().positive('Price must be greater than 0'),
  description: z.string().optional(),
  status: z.enum(['Active', 'SoldOut', 'Draft']),
});

type FormInput = z.input<typeof schema>;
type FormValues = z.output<typeof schema>;

const DEFAULTS: FormInput = { name: '', categoryId: 0, price: 0, description: '', status: 'Draft' };

interface AccessoryFormDialogProps {
  open: boolean;
  accessoryId: number | null;
  onClose: () => void;
}

export default function AccessoryFormDialog({ open, accessoryId, onClose }: AccessoryFormDialogProps) {
  const isEditing = accessoryId !== null;
  const { data: existing } = useAdminAccessory(accessoryId);
  const { data: categories } = useCategories();
  const createMutation = useCreateAccessory();
  const updateMutation = useUpdateAccessory();

  const [compatibleMobiles, setCompatibleMobiles] = useState<CompatibleMobile[]>([]);
  const [newBrand, setNewBrand] = useState('');
  const [newModel, setNewModel] = useState('');

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
              name: existing.name,
              categoryId: existing.categoryId,
              price: existing.price,
              description: existing.description ?? '',
              status: existing.status,
            }
          : DEFAULTS,
      );
      setCompatibleMobiles(existing?.compatibleMobiles ?? []);
    }
  }, [open, existing, reset]);

  const mutation = isEditing ? updateMutation : createMutation;

  const addCompatibleMobile = () => {
    if (newBrand.trim() && newModel.trim()) {
      setCompatibleMobiles((prev) => [...prev, { brand: newBrand.trim(), model: newModel.trim() }]);
      setNewBrand('');
      setNewModel('');
    }
  };

  const removeCompatibleMobile = (index: number) => {
    setCompatibleMobiles((prev) => prev.filter((_, i) => i !== index));
  };

  const onSubmit = (values: FormValues) => {
    const request: AccessoryRequest = { ...values, compatibleMobiles };
    if (isEditing) {
      updateMutation.mutate({ id: accessoryId, request }, { onSuccess: onClose });
    } else {
      createMutation.mutate(request, { onSuccess: onClose });
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEditing ? 'Edit Accessory' : 'Add Accessory'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField label="Name" fullWidth {...register('name')} error={!!errors.name} helperText={errors.name?.message}
            slotProps={{ inputLabel: { shrink: true } }} />
          <Stack direction="row" spacing={2}>
            <Controller
              name="categoryId"
              control={control}
              render={({ field }) => (
                <TextField select label="Category" fullWidth {...field} error={!!errors.categoryId} helperText={errors.categoryId?.message}>
                  <MenuItem value={0} disabled>
                    Select a category
                  </MenuItem>
                  {categories?.map((c) => (
                    <MenuItem key={c.categoryId} value={c.categoryId}>
                      {c.name}
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

          <Divider />
          <Typography variant="subtitle2">Compatible mobiles</Typography>
          <Stack direction="row" spacing={1} useFlexGap sx={{ flexWrap: 'wrap' }}>
            {compatibleMobiles.map((cm, i) => (
              <Chip key={i} label={`${cm.brand} ${cm.model}`} onDelete={() => removeCompatibleMobile(i)} size="small" />
            ))}
          </Stack>
          <Stack direction="row" spacing={1}>
            <TextField label="Brand" size="small" value={newBrand} onChange={(e) => setNewBrand(e.target.value)} />
            <TextField label="Model" size="small" value={newModel} onChange={(e) => setNewModel(e.target.value)} />
            <Button onClick={addCompatibleMobile}>Add</Button>
          </Stack>

          {mutation.isError && <Alert severity="error">Failed to save. Check the fields and try again.</Alert>}

          {isEditing && existing && (
            <>
              <Divider />
              <ImageManager
                ownerType="accessories"
                ownerId={accessoryId}
                imageIds={existing.imageIds}
                primaryImageId={existing.imageIds[0] ?? null}
                invalidateKey={['admin-accessory', accessoryId]}
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
