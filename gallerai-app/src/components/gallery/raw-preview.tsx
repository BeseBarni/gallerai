import React from 'react'
import { useImageStore } from '@/store/useImageStore'

interface RawPreviewProps {
  id: string
}

export const RawPreview: React.FC<RawPreviewProps> = ({ id }) => {
  const image = useImageStore((state) => state.images[id])

  if (!image) return null

  const { localUrl, status } = image
  const isError = status === 'error'

  if (isError) {
    return (
      <div className="bg-card flex flex-col items-center space-y-2 rounded-lg border border-red-200 p-2">
        <div className="flex aspect-square w-full items-center justify-center bg-red-50 text-xs text-red-500">
          Failed
        </div>
      </div>
    )
  }

  return (
    <div className="bg-card flex flex-col items-center space-y-2 rounded-lg border p-2">
      <div className="bg-muted relative aspect-square w-full overflow-hidden rounded-md">
        {/* CASE 1: LOADING (No URL yet) */}
        {!localUrl ? (
          <div className="flex h-full animate-pulse flex-col items-center justify-center space-y-1">
            <span className="text-muted-foreground text-xs font-medium">
              {status === 'waiting' ? 'Queued' : 'Processing...'}
            </span>
          </div>
        ) : (
          <img
            src={localUrl}
            className="h-full w-full object-cover transition-opacity duration-300"
            loading="lazy"
          />
        )}
      </div>
    </div>
  )
}
