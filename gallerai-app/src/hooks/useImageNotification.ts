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
      const { imageId, score, detailed_critique } = update
      console.log('Received image update:', update, { imageId, score, detailed_critique })
      updateImage(imageId, { score, critique: detailed_critique })
    }

    signalRManager.on('ReceiveImageNotification', handleImageUpdate)

    return () => {
      signalRManager.off('ReceiveImageNotification')
    }
  }, [updateImage])
}
