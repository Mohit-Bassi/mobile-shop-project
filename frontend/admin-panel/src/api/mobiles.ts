import { apiClient } from './client';
import type { MobileDetail, MobileListItem, MobileRequest, PagedResult } from '../types/api';

export interface AdminMobileFilters {
  status?: string;
  brand?: string;
  page?: number;
  pageSize?: number;
}

export async function fetchAdminMobiles(filters: AdminMobileFilters): Promise<PagedResult<MobileListItem>> {
  const { data } = await apiClient.get<PagedResult<MobileListItem>>('/admin/mobiles', { params: filters });
  return data;
}

export async function fetchAdminMobile(id: number): Promise<MobileDetail> {
  const { data } = await apiClient.get<MobileDetail>(`/admin/mobiles/${id}`);
  return data;
}

export async function createMobile(request: MobileRequest): Promise<{ mobileId: number }> {
  const { data } = await apiClient.post<{ mobileId: number }>('/admin/mobiles', request);
  return data;
}

export async function updateMobile(id: number, request: MobileRequest): Promise<void> {
  await apiClient.put(`/admin/mobiles/${id}`, request);
}

export async function updateMobileStatus(id: number, status: string): Promise<void> {
  await apiClient.patch(`/admin/mobiles/${id}/status`, { status });
}

export async function deleteMobile(id: number): Promise<void> {
  await apiClient.delete(`/admin/mobiles/${id}`);
}
