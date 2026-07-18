import { useQuery } from '@tanstack/react-query';
import { fetchMobileById, fetchMobiles, type MobileFilters } from '../api/mobiles';

export function useMobiles(filters: MobileFilters) {
  return useQuery({
    queryKey: ['mobiles', filters],
    queryFn: () => fetchMobiles(filters),
  });
}

export function useMobile(id: number) {
  return useQuery({
    queryKey: ['mobile', id],
    queryFn: () => fetchMobileById(id),
    enabled: Number.isFinite(id),
  });
}
