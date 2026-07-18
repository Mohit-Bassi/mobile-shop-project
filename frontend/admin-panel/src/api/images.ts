import { apiClient } from './client';
import type { ImageUploadResult } from '../types/api';

export type ImageOwnerType = 'mobiles' | 'accessories';

export async function uploadImage(ownerType: ImageOwnerType, ownerId: number, file: File): Promise<ImageUploadResult> {
  const formData = new FormData();
  formData.append('file', file);
  const { data } = await apiClient.post<ImageUploadResult>(`/admin/${ownerType}/${ownerId}/images`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return data;
}

export async function deleteImage(imageId: number): Promise<void> {
  await apiClient.delete(`/admin/images/${imageId}`);
}

export async function setPrimaryImage(imageId: number): Promise<void> {
  await apiClient.patch(`/admin/images/${imageId}/set-primary`);
}
