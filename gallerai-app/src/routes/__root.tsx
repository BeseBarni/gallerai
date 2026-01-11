import { TanStackRouterDevtools } from '@/utils/dev-tools'
import { createRootRoute, Outlet } from '@tanstack/react-router'

export const Route = createRootRoute({
  component: RootComponent,
})

function RootComponent() {
  return (
    <main>
      <Outlet />
      <TanStackRouterDevtools />
    </main>
  )
}
