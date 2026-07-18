import { apiClient } from './client';
import type { Inquiry, PagedResult } from '../types/api';

export interface AdminInquiryFilters {
  status?: string;
  page?: number;
  pageSize?: number;
}

export async function fetchInquiries(filters: AdminInquiryFilters): Promise<PagedResult<Inquiry>> {
  const { data } = await apiClient.get<PagedResult<Inquiry>>('/admin/inquiries', { params: filters });
  return data;
}

export async function updateInquiryStatus(id: number, status: string): Promise<void> {
  await apiClient.patch(`/admin/inquiries/${id}/status`, { status });
}
