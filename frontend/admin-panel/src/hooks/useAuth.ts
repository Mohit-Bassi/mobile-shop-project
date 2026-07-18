import { useMutation } from '@tanstack/react-query';
import { login as loginApi, logout as logoutApi } from '../api/auth';
import { useAuthStore } from '../store/authStore';

export function useLogin() {
  const setAuth = useAuthStore((s) => s.setAuth);
  return useMutation({
    mutationFn: ({ email, password }: { email: string; password: string }) => loginApi(email, password),
    onSuccess: (data) => setAuth(data.accessToken, data.expiresAtUtc),
  });
}

export function useLogout() {
  const clearAuth = useAuthStore((s) => s.clearAuth);
  return useMutation({
    mutationFn: logoutApi,
    onSettled: () => clearAuth(),
  });
}
