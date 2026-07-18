import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  createAccessory,
  deleteAccessory,
  fetchAdminAccessories,
  fetchAdminAccessory,
  updateAccessory,
  updateAccessoryStatus,
  type AdminAccessoryFilters,
} from '../api/accessories';
import type { AccessoryRequest } from '../types/api';

export function useAdminAccessories(filters: AdminAccessoryFilters) {
  return useQuery({ queryKey: ['admin-accessories', filters], queryFn: () => fetchAdminAccessories(filters) });
}

export function useAdminAccessory(id: number | null) {
  return useQuery({
    queryKey: ['admin-accessory', id],
    queryFn: () => fetchAdminAccessory(id as number),
    enabled: id !== null,
  });
}

export function useCreateAccessory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: AccessoryRequest) => createAccessory(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-accessories'] }),
  });
}

export function useUpdateAccessory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, request }: { id: number; request: AccessoryRequest }) => updateAccessory(id, request),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['admin-accessories'] });
      queryClient.invalidateQueries({ queryKey: ['admin-accessory', variables.id] });
    },
  });
}

export function useUpdateAccessoryStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) => updateAccessoryStatus(id, status),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-accessories'] }),
  });
}

export function useDeleteAccessory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteAccessory(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-accessories'] }),
  });
}
