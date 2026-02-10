import React, { useEffect } from 'react'
import { useImageStore } from '@/store/useImageStore'

interface RawPreviewProps {
  id: string
}

export const RawPreview: React.FC<RawPreviewProps> = ({ id }) => {
  const image = useImageStore((state) => state.images[id])

  useEffect(() => {
    console.log('RawPreview received image update:', { image })
  }, [image])
  if (!image) return null

  const { localUrl, status, critique, score } = image

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
    <div className="bg-card flex flex-col space-y-3 rounded-lg border p-2">
      {/* 1. IMAGE CONTAINER (Fixed Aspect Ratio) */}
      <div className="bg-muted relative aspect-square w-full overflow-hidden rounded-md">
        {!localUrl ? (
          <div className="flex h-full animate-pulse flex-col items-center justify-center space-y-1">
            <span className="text-muted-foreground text-xs font-medium">
              {status === 'waiting' ? 'Queued' : 'Processing...'}
            </span>
          </div>
        ) : (
          <>
            {score !== undefined && (
              <div className="absolute top-2 right-2 z-10 flex h-8 w-8 items-center justify-center rounded-full border border-white/20 bg-black/60 backdrop-blur-md">
                <span className="text-xs font-bold text-white">{score}</span>
              </div>
            )}
            <img
              src={localUrl}
              className="h-full w-full object-cover transition-opacity duration-300"
              alt={`Preview of ${id}`}
              loading="lazy"
            />
          </>
        )}
      </div>

      {/* 2. CRITIQUE SECTION (Outside the square) */}
      {critique && (
        <div className="space-y-1 px-1 pb-1">
          <h4 className="text-muted-foreground text-[10px] font-bold tracking-wider uppercase">
            AI Critique
          </h4>
          <p className="text-foreground/80 text-sm leading-relaxed italic">"{critique}"</p>
        </div>
      )}
    </div>
  )
}
