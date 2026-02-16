import { Suspense, use, useEffect, useMemo } from 'react'
import { useFolderStore } from '@/store/useFolderStore'
import { useImageStore } from '@/store/useImageStore'
import { useGetFolderImagesEndpointSuspense } from '@shared/src/api/gallerai/api.gen'
import { GalleraiDomainEnumsImageStatus } from '@shared/src/api/schemas'
import { useShallow } from 'zustand/react/shallow'

import { FolderViewContext } from '../folder-view/context'
import { RawPreview } from '../raw-preview'
import FolderImagesFallback from './folder-images-fallback'

function FolderImages() {
  const { id } = useFolderStore((state) => state.activeFolder!)
  const { setProcessedImageCount, setImageCount } = use(FolderViewContext)

  const imageQuery = useGetFolderImagesEndpointSuspense(id, {
    query: {
      queryKey: ['folderImages', id],
    },
  })

  const uploadingImages = useImageStore(
    useShallow((state) => Object.values(state.images).filter((p) => p.folderId === id)),
  )
  const serverImages = useMemo(() => imageQuery.data?.value?.images ?? [], [imageQuery.data])

  const displayImages = useMemo(() => {
    const combinedImages = [...serverImages, ...uploadingImages]
    return combinedImages
  }, [serverImages, uploadingImages])

  useEffect(() => {
    setImageCount(displayImages.length)
    const processed = displayImages.filter(
      (img) => img.status === GalleraiDomainEnumsImageStatus.READY || img.critique,
    ).length
    setProcessedImageCount(processed)
  }, [displayImages, setProcessedImageCount, setImageCount])

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
