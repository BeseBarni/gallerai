import { useEffect } from 'react'

import { env } from '@/lib/env'
import { signalRManager } from '@/lib/signalr-manager'

export const useSignalRConnection = () => {
  useEffect(() => {
    const maxAttempts = env.SIGNALR_RETRY_ATTEMPTS
    const retryDelay = env.SIGNALR_RETRY_DELAY_MS

    const connectWithRetry = async (attempt: number = 0) => {
      try {
        await signalRManager.start()
      } catch (err) {
        if (attempt < maxAttempts) {
          const delay = retryDelay * Math.pow(2, attempt)
          console.warn(
            `SignalR connection failed. Retrying in ${delay}ms... (Attempt ${attempt + 1}/${maxAttempts})`,
          )

          setTimeout(() => connectWithRetry(attempt + 1), delay)
        } else {
          console.error('SignalR connection failed after maximum retries:', err)
        }
      }
    }
    connectWithRetry()
  }, [])
}
