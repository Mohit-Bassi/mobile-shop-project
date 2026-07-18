import { apiClient } from './client';
import type { MobileDetail, MobileListItem, PagedResult } from '../types/api';

export interface MobileFilters {
  brand?: string;
  minPrice?: number;
  maxPrice?: number;
  condition?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export async function fetchMobiles(filters: MobileFilters): Promise<PagedResult<MobileListItem>> {
  const { data } = await apiClient.get<PagedResult<MobileListItem>>('/mobiles', { params: filters });
  return data;
}

export async function fetchMobileById(id: number): Promise<MobileDetail> {
  const { data } = await apiClient.get<MobileDetail>(`/mobiles/${id}`);
  return data;
}
