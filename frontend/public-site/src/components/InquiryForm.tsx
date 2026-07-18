import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Alert, Box, Button, Stack, TextField } from '@mui/material';
import { useSubmitInquiry } from '../hooks/useSubmitInquiry';
import type { InquiryListingType } from '../types/api';

const schema = z.object({
  customerName: z.string().min(1, 'Name is required').max(150),
  customerPhone: z.string().min(1, 'Phone number is required').max(30),
  customerEmail: z.string().email('Enter a valid email').max(256).optional().or(z.literal('')),
  message: z.string().max(1000).optional(),
});

type FormValues = z.infer<typeof schema>;

interface InquiryFormProps {
  listingType: InquiryListingType;
  listingId?: number;
}

export default function InquiryForm({ listingType, listingId }: InquiryFormProps) {
  const [submitted, setSubmitted] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  const mutation = useSubmitInquiry();

  const onSubmit = (values: FormValues) => {
    mutation.mutate(
      {
        listingType,
        listingId,
        customerName: values.customerName,
        customerPhone: values.customerPhone,
        customerEmail: values.customerEmail || undefined,
        message: values.message || undefined,
      },
      {
        onSuccess: () => {
          reset();
          setSubmitted(true);
        },
      },
    );
  };

  if (submitted) {
    return <Alert severity="success">Thanks! We've received your inquiry and will get back to you soon.</Alert>;
  }

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
      <Stack spacing={2}>
        <TextField
          label="Your name"
          required
          fullWidth
          {...register('customerName')}
          error={!!errors.customerName}
          helperText={errors.customerName?.message}
        />
        <TextField
          label="Phone number"
          required
          fullWidth
          {...register('customerPhone')}
          error={!!errors.customerPhone}
          helperText={errors.customerPhone?.message}
        />
        <TextField
          label="Email (optional)"
          fullWidth
          {...register('customerEmail')}
          error={!!errors.customerEmail}
          helperText={errors.customerEmail?.message}
        />
        <TextField
          label="Message (optional)"
          fullWidth
          multiline
          minRows={3}
          {...register('message')}
          error={!!errors.message}
          helperText={errors.message?.message}
        />
        {mutation.isError && <Alert severity="error">Something went wrong. Please try again.</Alert>}
        <Button type="submit" variant="contained" disabled={mutation.isPending}>
          {mutation.isPending ? 'Sending…' : 'Send Inquiry'}
        </Button>
      </Stack>
    </Box>
  );
}
