import React from 'react'
import type { GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto } from '@shared/src/api/schemas/galleraiApplicationFeaturesFoldersGetFolderImagesImageDto'

interface RawPreviewProps {
  image: GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto
}

export const RawPreview: React.FC<RawPreviewProps> = ({ image }) => {
  if (!image) return null

  const { cdnUrl, status, aestheticScore } = image

  return (
    <div className="bg-card flex flex-col space-y-3 rounded-lg border p-2">
      {/* 1. IMAGE CONTAINER (Fixed Aspect Ratio) */}
      <div className="bg-muted relative aspect-square w-full overflow-hidden rounded-md">
        {!cdnUrl ? (
          <div className="flex h-full animate-pulse flex-col items-center justify-center space-y-1">
            <span className="text-muted-foreground text-xs font-medium">
              {status === 0 ? 'Queued' : 'Processing...'}
            </span>
          </div>
        ) : (
          <>
            {aestheticScore !== undefined && (
              <div className="absolute top-2 right-2 z-10 flex h-8 w-8 items-center justify-center rounded-full border border-white/20 bg-black/60 backdrop-blur-md">
                <span className="text-xs font-bold text-white">{aestheticScore}</span>
              </div>
            )}
            <img
              src={cdnUrl}
              className="h-full w-full object-cover transition-opacity duration-300"
              alt={`Preview of ${image.imageId}`}
              loading="lazy"
            />
          </>
        )}
      </div>
    </div>
  )
}
