import { useImageStore } from '@/store/useImageStore'

import { signalRManager } from './signalr-manager'

let isInitialized = false
export const InitSignalRBridge = () => {
  if (isInitialized) return
  isInitialized = true

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  signalRManager.subscribe('ImageUpdate', (update: any) => {
    const updateImage = useImageStore.getState().updateImage

    const { imageId, score } = update
    updateImage(imageId, { aestheticScore: score * 10, critique: update.detailed_critique })
  })
}
