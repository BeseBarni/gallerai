import { useImageStore } from '@/store/useImageStore'
import { imageProcessor } from '@/workers/image/worker-pool'

export const startImagePipeline = async (id: string) => {
  const store = useImageStore.getState()
  const image = store.images[id]
  if (!image) return

  try {
    store.updateImage(id, { status: 'developing' })
    const processedData = await imageProcessor.process(image.file)

    const standardBuffer = new Uint8Array(processedData.length)
    standardBuffer.set(processedData)

    const blob = new Blob([standardBuffer], { type: 'image/jpeg' })
    const localUrl = URL.createObjectURL(blob)
    store.updateImage(id, { localUrl, status: 'uploading' })

    store.updateImage(id, { status: 'ai_processing' })
  } catch (error) {
    store.updateImage(id, { status: 'error' })
    console.error('Error in image pipeline:', error)
  }
}
