import { useImageStore } from '@/store/useImageStore'
import { createFileRoute } from '@tanstack/react-router'
import { useShallow } from 'zustand/react/shallow'

import { startImagePipeline } from '@/lib/image-processing/orchestrator'
import { FileUploadZone } from '@/components/gallery/file-upload-zone'
import { RawPreview } from '@/components/gallery/raw-preview'

export const Route = createFileRoute('/upload')({
  component: RouteComponent,
})

function RouteComponent() {
  const imageIds = useImageStore(useShallow((state) => Object.keys(state.images)))

  const onUpload = (newFiles: File[]) => {
    newFiles.forEach((file) => {
      const id = crypto.randomUUID()

      useImageStore.getState().addImage({
        id,
        status: 'waiting',
        localUrl: null,
      })

      startImagePipeline(id, file)
    })
  }

  return (
    <div className="bg-background flex min-h-screen flex-col items-center justify-center space-y-4 p-2">
      <FileUploadZone onFilesSelected={onUpload} />
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        {imageIds.map((imageId, index) => (
          <RawPreview key={`${imageId}-${index}`} id={imageId} />
        ))}
      </div>
    </div>
  )
}
