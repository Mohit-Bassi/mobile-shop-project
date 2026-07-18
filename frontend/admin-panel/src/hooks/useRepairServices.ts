import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { createRepairService, deleteRepairService, fetchRepairServices, updateRepairService } from '../api/repairServices';
import type { RepairServiceRequest } from '../types/api';

export function useRepairServices() {
  return useQuery({ queryKey: ['admin-repair-services'], queryFn: fetchRepairServices });
}

export function useCreateRepairService() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: RepairServiceRequest) => createRepairService(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-repair-services'] }),
  });
}

export function useUpdateRepairService() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, request }: { id: number; request: RepairServiceRequest }) => updateRepairService(id, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-repair-services'] }),
  });
}

export function useDeleteRepairService() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteRepairService(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-repair-services'] }),
  });
}
