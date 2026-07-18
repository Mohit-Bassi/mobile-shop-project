import { apiClient } from './client';
import type { AccessoryDetail, AccessoryListItem, AccessoryRequest, PagedResult } from '../types/api';

export interface AdminAccessoryFilters {
  status?: string;
  categoryId?: number;
  page?: number;
  pageSize?: number;
}

export async function fetchAdminAccessories(filters: AdminAccessoryFilters): Promise<PagedResult<AccessoryListItem>> {
  const { data } = await apiClient.get<PagedResult<AccessoryListItem>>('/admin/accessories', { params: filters });
  return data;
}

export async function fetchAdminAccessory(id: number): Promise<AccessoryDetail> {
  const { data } = await apiClient.get<AccessoryDetail>(`/admin/accessories/${id}`);
  return data;
}

export async function createAccessory(request: AccessoryRequest): Promise<{ accessoryId: number }> {
  const { data } = await apiClient.post<{ accessoryId: number }>('/admin/accessories', request);
  return data;
}

export async function updateAccessory(id: number, request: AccessoryRequest): Promise<void> {
  await apiClient.put(`/admin/accessories/${id}`, request);
}

export async function updateAccessoryStatus(id: number, status: string): Promise<void> {
  await apiClient.patch(`/admin/accessories/${id}/status`, { status });
}

export async function deleteAccessory(id: number): Promise<void> {
  await apiClient.delete(`/admin/accessories/${id}`);
}
