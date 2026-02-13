import { Button } from '@/shadcn/button'
import { useFolderStore } from '@/store/useFolderStore'
import { useNavigate } from '@tanstack/react-router'
import { ArrowLeft } from 'lucide-react'

import { startImagePipeline } from '@/lib/image-processing/orchestrator'

import { FileUploadZone } from './file-upload-zone'
import FolderImagesView from './folder-images/folder-images'

export default function FolderView() {
  const { activeFolder, setActiveFolder } = useFolderStore()

  const Navigate = useNavigate()

  const onUpload = (newFiles: File[]) => {
    newFiles.forEach((file) => {
      const id = crypto.randomUUID()
      startImagePipeline(id, activeFolder!.id, file)
    })
  }

  return (
    <div className="animate-in fade-in flex h-full flex-col space-y-6 p-8 duration-300">
      <div className="flex items-center space-x-4">
        <Button
          variant="outline"
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
      </div>
      <FolderImagesView />
      <FileUploadZone onFilesSelected={onUpload} />
    </div>
  )
}
