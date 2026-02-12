import { useMemo } from 'react'
import { Button } from '@/shadcn/button'
import { useImageStore } from '@/store/useImageStore'
import { useGetFolderImagesEndpoint } from '@shared/src/api/gallerai/api.gen'
import { ArrowLeft, Loader2 } from 'lucide-react' // Added Loader2
import { useShallow } from 'zustand/shallow'

import { startImagePipeline } from '@/lib/image-processing/orchestrator'

import { FileUploadZone } from './file-upload-zone'
import { RawPreview } from './raw-preview'

export default function FolderView({
  folderId,
  name,
  setActiveFolderId,
}: {
  folderId: string // Removed '?' since you handle the check in parent
  name: string
  setActiveFolderId: (id: string | null) => void
}) {
  const imageQuery = useGetFolderImagesEndpoint(folderId, {
    query: {
      queryKey: ['folderImages', folderId],
      enabled: !!folderId,
    },
  })

  // DEBUG: Check what the API actually returns
  console.log('FolderView Data:', {
    isLoading: imageQuery.isLoading,
    data: imageQuery.data,
    value: imageQuery.data?.value,
  })

  const uploadingImages = useImageStore(
    useShallow((state) => Object.values(state.images).filter((p) => p.folderId === folderId)),
  )

  const displayImages = useMemo(() => {
    // Check if the path is correct.
    // It might be data?.images, data?.value, or data?.value?.images
    const serverImages = imageQuery.data?.value?.images ?? []
    return [...uploadingImages, ...serverImages]
  }, [imageQuery.data, uploadingImages])

  const onUpload = (newFiles: File[]) => {
    newFiles.forEach((file) => {
      const id = crypto.randomUUID()
      // Ensure folderId is passed correctly
      startImagePipeline(id, folderId, file)
    })
  }

  return (
    <div className="animate-in fade-in flex h-full flex-col space-y-6 p-8 duration-300">
      <div className="flex items-center space-x-4">
        <Button variant="outline" size="icon" onClick={() => setActiveFolderId(null)}>
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <div>
          <h2 className="text-2xl font-bold tracking-tight">{name}</h2>
          <p className="text-muted-foreground">Manage assets for this folder</p>
        </div>
      </div>

      <FileUploadZone onFilesSelected={onUpload} />

      {/* Handle Loading State */}
      {imageQuery.isLoading ? (
        <div className="text-muted-foreground flex h-40 w-full items-center justify-center">
          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
          Loading images...
        </div>
      ) : displayImages.length === 0 ? (
        <div className="text-muted-foreground flex h-40 w-full items-center justify-center rounded-lg border border-dashed">
          No images found in this folder.
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
          {displayImages.map((image, index) => (
            // Ensure 'image.imageId' exists. If API returns 'id', use 'image.id'
            <RawPreview key={`${image.imageId || image.imageId || index}`} image={image} />
          ))}
        </div>
      )}
    </div>
  )
}
