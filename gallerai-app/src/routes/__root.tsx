import { TanStackRouterDevtools } from '@/utils/dev-tools'
import { createRootRoute, Outlet } from '@tanstack/react-router'

import GalleraiSplash from '@/components/ui/gallerai-splash'

export const Route = createRootRoute({
  component: RootComponent,

  pendingComponent: () => <GalleraiSplash />,
})

function RootComponent() {
  return (
    <main>
      <Outlet />
      <TanStackRouterDevtools />
    </main>
  )
}
