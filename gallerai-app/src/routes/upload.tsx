import { useState } from 'react'
import { createFileRoute } from '@tanstack/react-router'

import { FileUploadZone } from '@/components/gallery/file-upload-zone'
import { RawPreview } from '@/components/gallery/raw-preview'

export const Route = createFileRoute('/upload')({
  component: RouteComponent,
})

function RouteComponent() {
  const [files, setFiles] = useState<File[]>([])

  const onUpload = (files: File[]) => {
    setFiles((prev) => [...prev, ...files])
  }
  return (
    <div className="bg-background flex min-h-screen flex-col items-center justify-center space-y-4 p-2">
      <FileUploadZone onFilesSelected={onUpload} />
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        {files.map((file, index) => (
          <RawPreview key={`${file.name}-${index}`} file={file} />
        ))}
      </div>
    </div>
  )
}
