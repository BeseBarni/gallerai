import { imagePresignedUrl } from '@/api/gallerai.gen'
import { useImageStore } from '@/store/useImageStore'
import { uploadFileWithProgress } from '@/utils/upload-helpers'
import { imageProcessor } from '@/workers/image/worker-pool'

export const startImagePipeline = async (id: string, file: File) => {
  const store = useImageStore.getState()
  console.log('Starting pipeline for', id, file)
  try {
    store.addImage({ id, localUrl: null, status: 'waiting' })
    const processedData = await imageProcessor.process(file)
    const type = 'image/jpeg'
    const blob = new Blob([processedData], { type: type })

    const localUrl = URL.createObjectURL(blob)
    store.updateImage(id, { localUrl, status: 'uploading' })

    const result = await imagePresignedUrl({
      key: id,
      fileName: `${id}.jpg`,
      contentType: 'image/jpeg',
    }).then((p) => p.value)
    if (!result?.uploadUrl) throw new Error('Failed to get upload URL')

    await uploadFileWithProgress({
      url: result.uploadUrl,
      file: blob,
      contentType: type,
      onProgress: () => {},
    })
    console.log('uplopaf', id, result)
    store.updateImage(id, {
      localUrl: result.cdnUrl,
      status: 'ai_processing',
    })
  } catch (error) {
    store.updateImage(id, { status: 'error' })
    console.error('Error in image pipeline:', error)
  }
}
