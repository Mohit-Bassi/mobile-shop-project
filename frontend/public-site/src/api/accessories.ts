import { apiClient } from './client';
import type { AccessoryDetail, AccessoryListItem, PagedResult } from '../types/api';

export interface AccessoryFilters {
  categoryId?: number;
  compatibleBrand?: string;
  compatibleModel?: string;
  page?: number;
  pageSize?: number;
}

export async function fetchAccessories(filters: AccessoryFilters): Promise<PagedResult<AccessoryListItem>> {
  const { data } = await apiClient.get<PagedResult<AccessoryListItem>>('/accessories', { params: filters });
  return data;
}

export async function fetchAccessoryById(id: number): Promise<AccessoryDetail> {
  const { data } = await apiClient.get<AccessoryDetail>(`/accessories/${id}`);
  return data;
}
