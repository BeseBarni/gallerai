// src/routes/_authenticated.tsx
import { useAuthStore } from '@/store/useAuthStore'
import { createFileRoute, Outlet, redirect } from '@tanstack/react-router'

import { connectWithRetry } from '@/lib/signalr/signalr-manager'

export const Route = createFileRoute('/__authenticated')({
  beforeLoad: ({ location }) => {
    const user = useAuthStore.getState()

    if (!user.isAuthenticated || !user.token) {
      throw redirect({
        to: '/',
        search: {
          redirect: location.href,
        },
      })
    }

    connectWithRetry()
  },
  component: () => <Outlet />,
})
