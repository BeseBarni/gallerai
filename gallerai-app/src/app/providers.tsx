import { useEffect } from 'react'
import { ReactQueryDevtools } from '@/utils/dev-tools'
import { setAxiosInstance } from '@gallerai/shared/lib/api-client-base'
import { QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from '@tanstack/react-router'
import { ErrorBoundary } from 'react-error-boundary'
import { ToastContainer } from 'react-toastify'

import axiosInstance from '@/lib/api-client'
import { queryClient } from '@/lib/query-client'
import { router } from '@/lib/router'
import { InitSignalRBridge } from '@/lib/signalr/signalr-bridge'

export default function AppProvider() {
  setAxiosInstance(axiosInstance)

  useEffect(() => {
    InitSignalRBridge()
  }, [])

  return (
    <>
      <ErrorBoundary
        fallback={
          <div className="flex h-screen items-center justify-center">
            An unexpected error occurred.
          </div>
        }
      >
        <ToastContainer />
        <QueryClientProvider client={queryClient}>
          <RouterProvider router={router} />
          <ReactQueryDevtools />
        </QueryClientProvider>
      </ErrorBoundary>
    </>
  )
}
