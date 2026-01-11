import { createFileRoute } from '@tanstack/react-router'

import { FileUploadZone } from '@/components/gallery/file-upload-zone'

export const Route = createFileRoute('/upload')({
  component: RouteComponent,
})

function RouteComponent() {
  return (
    <div className="bg-background flex min-h-screen flex-col items-center justify-center space-y-8 p-6">
      <FileUploadZone
        onFilesSelected={(file) => {
          console.log(file)
        }}
      />
    </div>
  )
}
