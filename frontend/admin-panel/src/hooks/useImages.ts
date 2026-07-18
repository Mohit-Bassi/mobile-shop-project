import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteImage, setPrimaryImage, uploadImage, type ImageOwnerType } from '../api/images';

export function useUploadImage(invalidateKey: unknown[]) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ ownerType, ownerId, file }: { ownerType: ImageOwnerType; ownerId: number; file: File }) =>
      uploadImage(ownerType, ownerId, file),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: invalidateKey }),
  });
}

export function useDeleteImage(invalidateKey: unknown[]) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (imageId: number) => deleteImage(imageId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: invalidateKey }),
  });
}

export function useSetPrimaryImage(invalidateKey: unknown[]) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (imageId: number) => setPrimaryImage(imageId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: invalidateKey }),
  });
}
