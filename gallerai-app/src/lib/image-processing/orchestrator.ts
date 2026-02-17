import { useImageStore } from '@/store/useImageStore'
import { processImage } from '@/utils/image-helpers'
import { uploadFileWithProgress } from '@/utils/upload-helpers'
import { imagePresignedUrl } from '@gallerai/shared/web'

export const startImagePipeline = async (id: string, folderId: string, file: File) => {
  const store = useImageStore.getState()

  let localUrl: string | null = null

  try {
    store.addImage({ folderId, imageId: id, status: 0 })

    const blobToUpload = await processImage(file)

    localUrl = URL.createObjectURL(blobToUpload)

    store.updateImage(id, { cdnUrl: localUrl, status: 1 })

    const result = await imagePresignedUrl({
      key: id,
      fileName: `${id}.jpg`,
      contentType: 'image/jpeg',
      folderId: folderId,
    }).then((p) => p.value)

    if (!result?.uploadUrl) throw new Error('Failed to get upload URL')

    await uploadFileWithProgress({
      url: result.uploadUrl,
      file: blobToUpload,
      contentType: 'image/jpeg',
      traceparent: result.traceparent,
      onProgress: () => {},
    })

    store.updateImage(id, {
      cdnUrl: result.cdnUrl,
      status: 2,
    })
  } catch (error) {
    store.updateImage(id, { status: 4 })
    console.error('Error in image pipeline:', error)
  } finally {
    if (localUrl) {
      URL.revokeObjectURL(localUrl)
    }
  }
}
