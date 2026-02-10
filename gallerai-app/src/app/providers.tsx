import { ReactQueryDevtools } from '@/utils/dev-tools'
import { setAxiosInstance } from '@gallerai/shared/lib/api-client-base'
import { QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from '@tanstack/react-router'

import axiosInstance from '@/lib/api-client'
import { queryClient } from '@/lib/query-client'
import { router } from '@/lib/router'
import { useImageNotification } from '@/hooks/useImageNotification'
import { useSignalRConnection } from '@/hooks/useSignalRConnection'

export default function AppProvider() {
  setAxiosInstance(axiosInstance)

  useSignalRConnection()
  useImageNotification()

  return (
    <>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
        <ReactQueryDevtools />
      </QueryClientProvider>
    </>
  )
}
