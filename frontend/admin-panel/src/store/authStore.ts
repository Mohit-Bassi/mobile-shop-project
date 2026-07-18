import { create } from 'zustand';

interface AuthState {
  accessToken: string | null;
  expiresAtUtc: string | null;
  isAuthenticated: boolean;
  setAuth: (accessToken: string, expiresAtUtc: string) => void;
  clearAuth: () => void;
}

// Deliberately not persisted to localStorage/sessionStorage — the access token lives only in
// memory for this tab's lifetime; the refresh token (httpOnly cookie) is what survives a reload.
export const useAuthStore = create<AuthState>((set) => ({
  accessToken: null,
  expiresAtUtc: null,
  isAuthenticated: false,
  setAuth: (accessToken, expiresAtUtc) => set({ accessToken, expiresAtUtc, isAuthenticated: true }),
  clearAuth: () => set({ accessToken: null, expiresAtUtc: null, isAuthenticated: false }),
}));
