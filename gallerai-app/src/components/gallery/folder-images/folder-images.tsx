import { Suspense, use, useMemo } from 'react'
import { useFolderStore } from '@/store/useFolderStore'
import { useImageStore } from '@/store/useImageStore'
import { useGetFolderImagesEndpointSuspense } from '@shared/src/api/gallerai/api.gen'
import { useShallow } from 'zustand/react/shallow'

import { FolderViewContext } from '../folder-view/context'
import { RawPreview } from '../raw-preview'
import FolderImagesFallback from './folder-images-fallback'

function FolderImages() {
  const { id } = useFolderStore((state) => state.activeFolder!)
  const imageQuery = useGetFolderImagesEndpointSuspense(id, {
    query: {
      queryKey: ['folderImages', id],
    },
  })

  const { setProcessedImageCount, setImageCount } = use(FolderViewContext)
  const uploadingImages = useImageStore(
    useShallow((state) => Object.values(state.images).filter((p) => p.folderId === id)),
  )

  const displayImages = useMemo(() => {
    const serverImages = imageQuery.data?.value?.images ?? []
    const combinedImages = [...serverImages, ...uploadingImages]
    setImageCount(combinedImages.length)
    setProcessedImageCount(combinedImages.filter((img) => img.critique).length)
    return combinedImages
  }, [imageQuery.data?.value?.images, setImageCount, setProcessedImageCount, uploadingImages])

  return (
    <>
      {displayImages.map((image, index) => (
        <RawPreview key={`${image.imageId || index}`} image={image} />
      ))}
    </>
  )
}

export default function FolderImagesView() {
  return (
    <>
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        <Suspense fallback={<FolderImagesFallback />}>
          <FolderImages />
        </Suspense>
      </div>
    </>
  )
}
