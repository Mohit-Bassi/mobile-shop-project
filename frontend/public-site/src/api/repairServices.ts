import { apiClient } from './client';
import type { RepairService } from '../types/api';

export async function fetchRepairServices(): Promise<RepairService[]> {
  const { data } = await apiClient.get<RepairService[]>('/repair-services');
  return data;
}
