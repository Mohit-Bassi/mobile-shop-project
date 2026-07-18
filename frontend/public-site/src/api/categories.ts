import { apiClient } from './client';
import type { Category } from '../types/api';

export async function fetchCategories(): Promise<Category[]> {
  const { data } = await apiClient.get<Category[]>('/categories');
  return data;
}
