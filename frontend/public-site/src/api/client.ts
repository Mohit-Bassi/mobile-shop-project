import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
});

export function imageUrl(imageId: number, variant: 'thumbnail' | 'medium' | 'full' = 'medium'): string {
  return `${API_BASE_URL}/images/${imageId}/${variant}`;
}
