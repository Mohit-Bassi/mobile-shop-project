import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { createCategory, deleteCategory, fetchCategories, updateCategory } from '../api/categories';
import type { CategoryRequest } from '../types/api';

export function useCategories() {
  return useQuery({ queryKey: ['admin-categories'], queryFn: fetchCategories });
}

export function useCreateCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CategoryRequest) => createCategory(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-categories'] }),
  });
}

export function useUpdateCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, request }: { id: number; request: CategoryRequest }) => updateCategory(id, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-categories'] }),
  });
}

export function useDeleteCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteCategory(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-categories'] }),
  });
}
