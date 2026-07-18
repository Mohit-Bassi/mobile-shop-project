import { apiClient } from './client';
import type { Category, CategoryRequest } from '../types/api';

export async function fetchCategories(): Promise<Category[]> {
  const { data } = await apiClient.get<Category[]>('/admin/categories');
  return data;
}

export async function fetchCategory(id: number): Promise<Category> {
  const { data } = await apiClient.get<Category>(`/admin/categories/${id}`);
  return data;
}

export async function createCategory(request: CategoryRequest): Promise<{ categoryId: number }> {
  const { data } = await apiClient.post<{ categoryId: number }>('/admin/categories', request);
  return data;
}

export async function updateCategory(id: number, request: CategoryRequest): Promise<void> {
  await apiClient.put(`/admin/categories/${id}`, request);
}

export async function deleteCategory(id: number): Promise<void> {
  await apiClient.delete(`/admin/categories/${id}`);
}
