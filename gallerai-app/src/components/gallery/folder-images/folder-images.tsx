import { Suspense, use, useEffect, useMemo } from 'react'
import { queryKeys } from '@/consts/query.keys'
import { useFolderStore } from '@/store/useFolderStore'
import { useImageStore } from '@/store/useImageStore'
import {
  useGetFolderImagesEndpointSuspense,
  useRemoveImageEndpoint,
} from '@shared/src/api/gallerai/api.gen'
import { GalleraiSharedKernelEnumsImageStatus } from '@shared/src/api/schemas'
import { AnimatePresence, motion } from 'framer-motion'
import { useShallow } from 'zustand/react/shallow'

import { queryClient } from '@/lib/query-client'

import { FolderViewContext } from '../folder-view/context'
import { GalleraiImagePreview } from '../image-preview/gallerai-image-preview'
import FolderImagesFallback from './folder-images-fallback'

function FolderImages() {
  const { id } = useFolderStore((state) => state.activeFolder!)
  const { setProcessedImageCount, setImageCount } = use(FolderViewContext)

  const deleteImageMutation = useRemoveImageEndpoint()

  const onDelete = (imageId: string) => {
    deleteImageMutation.mutate(
      { imageId },
      {
        onSettled: () => {
          queryClient.invalidateQueries({ queryKey: queryKeys.folderImages(id) })
        },
      },
    )
  }

  const imageQuery = useGetFolderImagesEndpointSuspense(id, {
    query: {
      queryKey: ['folderImages', id],
    },
  })

  const uploadingImages = useImageStore(
    useShallow((state) => Object.values(state.images).filter((p) => p.folderId === id)),
  )
  const serverImages = useMemo(() => imageQuery.data?.images ?? [], [imageQuery.data])

  const displayImages = useMemo(() => {
    const combinedImages = [...serverImages, ...uploadingImages]
    return combinedImages
  }, [serverImages, uploadingImages])

  useEffect(() => {
    setImageCount(displayImages.length)
    console.log(
      'Updated image count:',
      displayImages.map((img) => `${img.imageId}: ${img.status}`).join(', '),
    )
    const processed = displayImages.filter(
      (img) => !!img.status && img.status === GalleraiSharedKernelEnumsImageStatus.READY,
    ).length
    setProcessedImageCount(processed)
  }, [displayImages, setProcessedImageCount, setImageCount])

  return (
    <AnimatePresence mode="popLayout">
      {displayImages.map((image) => (
        <motion.div
          key={`${image.imageId}`}
          layout
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          exit={{ opacity: 0, transition: { duration: 0.2 } }}
          transition={{
            type: 'spring',
            stiffness: 350,
            damping: 30,
            mass: 1,
          }}
        >
          <GalleraiImagePreview image={image} onDelete={onDelete} />
        </motion.div>
      ))}
    </AnimatePresence>
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
