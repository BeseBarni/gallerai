import { useImageStore } from '@/store/useImageStore'
import { processImage } from '@/utils/image-helpers'
import { uploadFileWithProgress } from '@/utils/upload-helpers'
import { imagePresignedUrl } from '@gallerai/shared/web'

export const startImagePipeline = async (id: string, file: File) => {
  const store = useImageStore.getState()

  let localUrl: string | null = null

  try {
    store.addImage({ id, localUrl: null, status: 'waiting' })

    const blobToUpload = await processImage(file)

    localUrl = URL.createObjectURL(blobToUpload)

    store.updateImage(id, { localUrl, status: 'uploading' })

    const result = await imagePresignedUrl({
      key: id,
      fileName: `${id}.jpg`,
      contentType: 'image/jpeg',
    }).then((p) => p.value)

    if (!result?.uploadUrl) throw new Error('Failed to get upload URL')

    await uploadFileWithProgress({
      url: result.uploadUrl,
      file: blobToUpload,
      contentType: 'image/jpeg',
      onProgress: () => {},
    })

    store.updateImage(id, {
      localUrl: result.cdnUrl,
      status: 'ai_processing',
    })
  } catch (error) {
    store.updateImage(id, { status: 'error' })
    console.error('Error in image pipeline:', error)
  } finally {
    if (localUrl) {
      URL.revokeObjectURL(localUrl)
    }
  }
}
