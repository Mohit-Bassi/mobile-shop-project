import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  createMobile,
  deleteMobile,
  fetchAdminMobile,
  fetchAdminMobiles,
  updateMobile,
  updateMobileStatus,
  type AdminMobileFilters,
} from '../api/mobiles';
import type { MobileRequest } from '../types/api';

export function useAdminMobiles(filters: AdminMobileFilters) {
  return useQuery({ queryKey: ['admin-mobiles', filters], queryFn: () => fetchAdminMobiles(filters) });
}

export function useAdminMobile(id: number | null) {
  return useQuery({
    queryKey: ['admin-mobile', id],
    queryFn: () => fetchAdminMobile(id as number),
    enabled: id !== null,
  });
}

export function useCreateMobile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: MobileRequest) => createMobile(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-mobiles'] }),
  });
}

export function useUpdateMobile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, request }: { id: number; request: MobileRequest }) => updateMobile(id, request),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['admin-mobiles'] });
      queryClient.invalidateQueries({ queryKey: ['admin-mobile', variables.id] });
    },
  });
}

export function useUpdateMobileStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) => updateMobileStatus(id, status),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-mobiles'] }),
  });
}

export function useDeleteMobile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteMobile(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-mobiles'] }),
  });
}
