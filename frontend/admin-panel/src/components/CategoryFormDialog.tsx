import { useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Alert, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, Stack, Switch, TextField } from '@mui/material';
import { useCreateCategory, useUpdateCategory } from '../hooks/useCategories';
import type { Category, CategoryRequest } from '../types/api';

const schema = z.object({
  name: z.string().min(1, 'Name is required').max(100),
  slug: z
    .string()
    .min(1, 'Slug is required')
    .max(120)
    .regex(/^[a-z0-9]+(-[a-z0-9]+)*$/, 'Lowercase, alphanumeric, hyphen-separated'),
  displayOrder: z.coerce.number().int(),
  isActive: z.boolean(),
});

type FormInput = z.input<typeof schema>;
type FormValues = z.output<typeof schema>;

const DEFAULTS: FormInput = { name: '', slug: '', displayOrder: 0, isActive: true };

interface CategoryFormDialogProps {
  open: boolean;
  category: Category | null;
  onClose: () => void;
}

export default function CategoryFormDialog({ open, category, onClose }: CategoryFormDialogProps) {
  const isEditing = category !== null;
  const createMutation = useCreateCategory();
  const updateMutation = useUpdateCategory();

  const {
    register,
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormInput, unknown, FormValues>({ resolver: zodResolver(schema), defaultValues: DEFAULTS });

  useEffect(() => {
    if (open) {
      reset(category ? { name: category.name, slug: category.slug, displayOrder: category.displayOrder, isActive: true } : DEFAULTS);
    }
  }, [open, category, reset]);

  const mutation = isEditing ? updateMutation : createMutation;

  const onSubmit = (values: FormValues) => {
    const request: CategoryRequest = { ...values };
    if (isEditing) {
      updateMutation.mutate({ id: category.categoryId, request }, { onSuccess: onClose });
    } else {
      createMutation.mutate(request, { onSuccess: onClose });
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{isEditing ? 'Edit Category' : 'Add Category'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField label="Name" fullWidth {...register('name')} error={!!errors.name} helperText={errors.name?.message}
            slotProps={{ inputLabel: { shrink: true } }} />
          <TextField label="Slug" fullWidth {...register('slug')} error={!!errors.slug} helperText={errors.slug?.message}
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
