import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { fetchInquiries, updateInquiryStatus, type AdminInquiryFilters } from '../api/inquiries';

export function useInquiries(filters: AdminInquiryFilters) {
  return useQuery({ queryKey: ['admin-inquiries', filters], queryFn: () => fetchInquiries(filters) });
}

export function useUpdateInquiryStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) => updateInquiryStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-inquiries'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard-summary'] });
    },
  });
}
