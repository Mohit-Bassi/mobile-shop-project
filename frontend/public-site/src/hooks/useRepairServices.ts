import { useQuery } from '@tanstack/react-query';
import { fetchRepairServices } from '../api/repairServices';

export function useRepairServices() {
  return useQuery({
    queryKey: ['repair-services'],
    queryFn: fetchRepairServices,
  });
}
