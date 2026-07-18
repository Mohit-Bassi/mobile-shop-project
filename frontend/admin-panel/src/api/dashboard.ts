import { apiClient } from './client';
import type { DashboardSummary } from '../types/api';

export async function fetchDashboardSummary(): Promise<DashboardSummary> {
  const { data } = await apiClient.get<DashboardSummary>('/admin/dashboard/summary');
  return data;
}
