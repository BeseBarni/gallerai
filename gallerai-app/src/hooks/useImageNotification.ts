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
      console.log('Received image update notification:', update)
      const { imageId, score } = update
      updateImage(imageId, { aestheticScore: score, critique: update.detailed_critique })
    }

    signalRManager.on('ImageUpdate', handleImageUpdate as (...args: unknown[]) => void)

    return () => {
      signalRManager.off('ImageUpdate')
    }
  }, [updateImage])
}
