import { useState } from 'react'
import { Button } from '@/shadcn/button'
import { useFolderStore } from '@/store/useFolderStore'
import { useNavigate } from '@tanstack/react-router'
import { ArrowLeft } from 'lucide-react'

import { startImagePipeline } from '@/lib/image-processing/orchestrator'

import { AIProcessCounter } from '../ai-process-counter'
import { FileUploadZone } from '../file-upload-zone'
import FolderImagesView from '../folder-images/folder-images'
import { FolderViewContext } from './context'

export default function FolderView() {
  const { activeFolder, setActiveFolder } = useFolderStore()
  const [processedImageCount, setProcessedImageCount] = useState(0)
  const [imageCount, setImageCount] = useState(activeFolder?.itemCount ?? 0)
  const Navigate = useNavigate()

  const onUpload = (newFiles: File[]) => {
    newFiles.forEach((file) => {
      const id = crypto.randomUUID()
      startImagePipeline(id, activeFolder!.id, file)
    })
  }

  return (
    <FolderViewContext.Provider
      value={{ foldedrId: activeFolder?.id ?? '', setProcessedImageCount, setImageCount }}
    >
      <div className="animate-in fade-in flex h-full flex-col space-y-6 p-8 duration-300">
        <div className="flex space-x-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => {
              setActiveFolder(null)
              Navigate({ to: '/dashboard' })
            }}
          >
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <div>
            <h2 className="text-2xl font-bold tracking-tight">{activeFolder!.name}</h2>
            <p className="text-muted-foreground">Manage assets for this folder</p>
          </div>
          <AIProcessCounter
            className="ml-auto"
            totalImages={imageCount}
            processedImages={processedImageCount}
          />
        </div>
        <FolderImagesView />
        <FileUploadZone onFilesSelected={onUpload} />
      </div>
    </FolderViewContext.Provider>
  )
}
