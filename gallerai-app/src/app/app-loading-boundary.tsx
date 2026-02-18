import { Suspense } from 'react'

import GalleraiSplash from '@/components/ui/gallerai-splash'

export function LoadingBoundary({ children }: { children: React.ReactNode }) {
  return (
    <>
      <Suspense fallback={<GalleraiSplash />}>{children}</Suspense>
    </>
  )
}
