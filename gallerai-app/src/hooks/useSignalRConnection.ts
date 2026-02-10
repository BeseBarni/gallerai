import { useEffect } from 'react'

import { signalRManager } from '@/lib/signalr-manager'

export const useSignalRConnection = () => {
  useEffect(() => {
    signalRManager.start()

    // return () => {
    //   signalRManager.stop()
    // }
  }, [])
}
