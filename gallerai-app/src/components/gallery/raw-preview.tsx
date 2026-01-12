import React, { useEffect, useState } from 'react'
import { isRaw } from '@/utils/image-helpers'
import { imageProcessor } from '@/workers/image/worker-pool'

interface RawPreviewProps {
  file: File
}

export const RawPreview: React.FC<RawPreviewProps> = ({ file }) => {
  const [url, setUrl] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let isCancelled = false
    let generatedUrl: string | null = null

    const processImage = async () => {
      try {
        if (!isRaw(file)) {
          const url = URL.createObjectURL(file)
          setUrl(url)
          return
        }

        const previewBuffer = await imageProcessor.process(file)

        const standardBuffer = new Uint8Array(previewBuffer.length)
        standardBuffer.set(previewBuffer)

        const blob = new Blob([standardBuffer], { type: 'image/jpeg' })

        if (!isCancelled) {
          generatedUrl = URL.createObjectURL(blob)
          setUrl(generatedUrl)
        }
      } catch (err) {
        if (!isCancelled) setError('Failed to process RAW')
        console.error(err)
      }
    }

    processImage()

    return () => {
      isCancelled = true
      if (generatedUrl) {
        URL.revokeObjectURL(generatedUrl)
      }
    }
  }, [file])

  if (error) return <div className="text-xs text-red-500">{error}</div>

  return (
    <div className="bg-card flex flex-col items-center space-y-2 rounded-lg border p-2">
      <div className="bg-muted relative aspect-square w-full overflow-hidden rounded-md">
        {!url ? (
          <div className="flex h-full animate-pulse items-center justify-center">
            <span className="text-muted-foreground text-xs">Processing...</span>
          </div>
        ) : (
          <img
            src={url}
            alt={file.name}
            className="h-full w-full object-cover transition-opacity duration-300"
          />
        )}
      </div>
      <p className="text-foreground w-full truncate text-center text-xs font-medium">{file.name}</p>
    </div>
  )
}
