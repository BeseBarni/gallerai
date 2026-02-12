import { useEffect } from 'react'
import { useImageStore } from '@/store/useImageStore'

import { signalRManager } from '@/lib/signalr-manager'

export const useImageNotification = () => {
  const updateImage = useImageStore((state) => state.updateImage)

  useEffect(() => {
    const handleImageUpdate = (update: {
      imageId: string
      score?: number
      detailed_critique?: string
    }) => {
      console.log('Received image update:', update)
      const { imageId, score } = update
      updateImage(imageId, { aestheticScore: score })
    }

    signalRManager.on('ReceiveImageNotification', handleImageUpdate as (...args: unknown[]) => void)

    return () => {
      signalRManager.off('ReceiveImageNotification')
    }
  }, [updateImage])
}
