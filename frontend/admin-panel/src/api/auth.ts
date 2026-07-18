import { apiClient } from './client';

export interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
}

export async function login(email: string, password: string): Promise<LoginResponse> {
  const { data } = await apiClient.post<LoginResponse>('/auth/login', { email, password });
  return data;
}

export async function logout(): Promise<void> {
  await apiClient.post('/auth/logout');
}

export async function refresh(): Promise<LoginResponse> {
  const { data } = await apiClient.post<LoginResponse>('/auth/refresh');
  return data;
}
