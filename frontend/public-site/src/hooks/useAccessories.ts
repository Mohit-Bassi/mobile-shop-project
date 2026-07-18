import { useQuery } from '@tanstack/react-query';
import { fetchAccessories, fetchAccessoryById, type AccessoryFilters } from '../api/accessories';

export function useAccessories(filters: AccessoryFilters) {
  return useQuery({
    queryKey: ['accessories', filters],
    queryFn: () => fetchAccessories(filters),
  });
}

export function useAccessory(id: number) {
  return useQuery({
    queryKey: ['accessory', id],
    queryFn: () => fetchAccessoryById(id),
    enabled: Number.isFinite(id),
  });
}
