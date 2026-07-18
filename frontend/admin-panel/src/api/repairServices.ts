import { apiClient } from './client';
import type { RepairService, RepairServiceRequest } from '../types/api';

export async function fetchRepairServices(): Promise<RepairService[]> {
  const { data } = await apiClient.get<RepairService[]>('/admin/repair-services');
  return data;
}

export async function fetchRepairService(id: number): Promise<RepairService> {
  const { data } = await apiClient.get<RepairService>(`/admin/repair-services/${id}`);
  return data;
}

export async function createRepairService(request: RepairServiceRequest): Promise<{ repairServiceId: number }> {
  const { data } = await apiClient.post<{ repairServiceId: number }>('/admin/repair-services', request);
  return data;
}

export async function updateRepairService(id: number, request: RepairServiceRequest): Promise<void> {
  await apiClient.put(`/admin/repair-services/${id}`, request);
}

export async function deleteRepairService(id: number): Promise<void> {
  await apiClient.delete(`/admin/repair-services/${id}`);
}
