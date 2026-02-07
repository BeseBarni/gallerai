import { imagePresignedUrl } from '@/api/gallerai.gen'
import { useImageStore } from '@/store/useImageStore'
import { isRaw } from '@/utils/image-helpers'
import { uploadFileWithProgress } from '@/utils/upload-helpers'
import { imageProcessor } from '@/workers/image/worker-pool'

export const startImagePipeline = async (id: string, file: File) => {
  const store = useImageStore.getState()

  try {
    store.addImage({ id, localUrl: null, status: 'waiting' })

    let imageData: ArrayBuffer | null = null

    let type = file.type
    if (!isRaw(file)) {
      imageData = await file.arrayBuffer()
    } else {
      imageData = await imageProcessor.process(file)
      type = 'image/jpeg'
    }

    const blob = new Blob([imageData], { type: type })

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

    store.updateImage(id, {
      localUrl: result.cdnUrl,
      status: 'ai_processing',
    })
  } catch (error) {
    store.updateImage(id, { status: 'error' })
    console.error('Error in image pipeline:', error)
  }
}
