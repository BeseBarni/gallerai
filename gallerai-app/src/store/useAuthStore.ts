import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface AuthState {
  token: string | null
  email: string | null
  userName: string | null
  isAuthenticated: boolean
  setAuth: (token: string, email: string, userName: string | null) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      email: null,
      userName: null,
      isAuthenticated: false,
      setAuth: (token, email, userName) =>
        set({
          token,
          email,
          userName,
          isAuthenticated: true,
        }),
      logout: () =>
        set({
          token: null,
          email: null,
          userName: null,
          isAuthenticated: false,
        }),
    }),
    {
      name: 'auth-storage',
    },
  ),
)
