import { memo } from 'react'
import {
  ContextMenu,
  ContextMenuContent,
  ContextMenuItem,
  ContextMenuTrigger,
} from '@/shadcn/context-menu'
import { GalleraiSharedKernelEnumsImageStatus } from '@shared/src/api/schemas'
import type { GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto } from '@shared/src/api/schemas/galleraiApplicationFeaturesFoldersGetFolderImagesImageDto'
import { Star, Trash2 } from 'lucide-react'

import { AICritiqueSection } from './ai-critique'
import { GalleraiImage } from './gallerai-image'
import { ImageCard } from './image-card'
import { AnalysisOverlay, ErrorOverlay, UploadingOverlay } from './overlays'

interface ExtendedImageDto extends GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto {
  isFavorite?: boolean
}

interface GalleraiImagePreviewProps {
  image: ExtendedImageDto
  onDelete?: (imageId: string) => void
  onRetry?: (imageId: string) => void
}

export const GalleraiImagePreview = memo(function RawPreview({
  image,
  onDelete,
  onRetry,
}: GalleraiImagePreviewProps) {
  if (!image) return null

  const { cdnUrl, status, aestheticScore, critique, imageId, isFavorite } = image

  const isAnalyzing = status === GalleraiSharedKernelEnumsImageStatus.ANALYZING
  const isUploading = status === GalleraiSharedKernelEnumsImageStatus.UPLOADING
  const isFailed = status === GalleraiSharedKernelEnumsImageStatus.ANALYSIS_ERROR

  const favoriteBadge = isFavorite ? (
    <div className="absolute top-2 left-2 z-20 flex h-8 w-8 items-center justify-center rounded-full bg-black/60 backdrop-blur-md">
      <Star className="h-6 w-6 fill-yellow-400 text-yellow-400" />
    </div>
  ) : null

  return (
    <ContextMenu>
      {/* The Trigger wraps the card so the whole area is interactive */}
      <ContextMenuTrigger>
        <ImageCard footer={<AICritiqueSection critique={critique} />} badges={favoriteBadge}>
          <GalleraiImage imageId={imageId} cdnUrl={cdnUrl} aestheticScore={aestheticScore} />
          {isUploading && <UploadingOverlay />}
          {isAnalyzing && <AnalysisOverlay />}
          {isFailed && <ErrorOverlay onRetry={() => imageId && onRetry?.(imageId)} />}
        </ImageCard>
      </ContextMenuTrigger>

      <ContextMenuContent>
        <ContextMenuItem
          className="text-red-600 focus:bg-red-50 focus:text-red-600"
          onClick={() => imageId && onDelete?.(imageId)}
        >
          <Trash2 className="mr-2 h-4 w-4" />
          Delete Image
        </ContextMenuItem>
      </ContextMenuContent>
    </ContextMenu>
  )
})
