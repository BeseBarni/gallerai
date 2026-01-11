import { UploadCloud } from 'lucide-react'
import { useDropzone, type Accept } from 'react-dropzone'

import { cn } from '@/lib/utils'

export type FileUploadZoneProps = {
  title?: string
  subTitle?: string
  accept?: Accept
  onFilesSelected: (files: File[]) => void
}

export function FileUploadZone({
  title = 'Drag & drop RAW images, or click to select',
  subTitle = 'Supports .ARW, .CR2, .NEF, .DNG',
  accept = { 'image/*': ['.raw', '.arw', '.cr2', '.nef', '.dng'] },
  onFilesSelected,
}: FileUploadZoneProps) {
  const onDrop = (acceptedFiles: File[]) => {
    onFilesSelected(acceptedFiles)
  }

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept,
  })

  return (
    <div
      {...getRootProps()}
      className={cn(
        'flex h-64 w-full cursor-pointer flex-col items-center justify-center rounded-xl border-2 border-dashed transition-all',
        isDragActive
          ? 'border-primary bg-primary/5 scale-[1.01]'
          : 'border-muted-foreground/25 hover:border-primary/50',
      )}
    >
      <input {...getInputProps()} />
      <div className="flex flex-col items-center gap-2">
        <UploadCloud
          className={cn(
            'h-12 w-12 transition-colors',
            isDragActive ? 'text-primary' : 'text-muted-foreground',
          )}
        />
        <p className="text-sm font-medium">{title}</p>
        <p className="text-muted-foreground text-xs">{subTitle}</p>
      </div>
    </div>
  )
}
