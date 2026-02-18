import { memo, type PropsWithChildren } from 'react'
import {
  ContextMenu,
  ContextMenuContent,
  ContextMenuItem,
  ContextMenuTrigger,
} from '@/shadcn/context-menu'
import { GalleraiSharedKernelEnumsImageStatus } from '@shared/src/api/schemas'
import type { GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto } from '@shared/src/api/schemas/galleraiApplicationFeaturesFoldersGetFolderImagesImageDto'
import { Loader2, Sparkles, Trash2 } from 'lucide-react'

import { AICritique } from './ai-critique'

interface RawPreviewProps {
  image: GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto
  onDelete?: (imageId: string) => void // New prop for handling deletion
}

type ImageCardProps = {
  footer?: React.ReactNode
} & PropsWithChildren

const ImageCard = ({ children, footer }: ImageCardProps) => {
  return (
    <div className="bg-card flex flex-col space-y-3 rounded-lg border p-2">
      <div className="bg-muted group relative aspect-square w-full overflow-hidden rounded-md">
        {children}
      </div>
      {footer}
    </div>
  )
}

type AICritiqueProps = {
  critique: string | undefined | null
}

const AICritiqueSection = memo(function ({ critique }: AICritiqueProps) {
  return <>{critique && <AICritique critique={critique} />}</>
})

const ProcessingSplash = () => {
  return (
    <div className="flex h-full animate-pulse flex-col items-center justify-center space-y-1">
      <span className="text-muted-foreground text-xs font-medium">{'Queued'}</span>
    </div>
  )
}

// --- OVERLAYS ---

const AnalysisOverlay = () => {
  return (
    <div className="absolute inset-0 z-20 flex flex-col items-center justify-center bg-black/40 backdrop-blur-[2px] transition-all duration-500">
      <div className="relative">
        <Sparkles className="text-primary h-8 w-8 animate-pulse drop-shadow-[0_0_8px_rgba(253,224,71,0.6)]" />
        <Sparkles className="absolute -top-2 -right-2 h-4 w-4 animate-bounce text-white opacity-80 duration-1000" />
      </div>
      <span className="mt-2 animate-pulse text-xs font-medium tracking-wide text-white/90">
        Analyzing...
      </span>
    </div>
  )
}

const UploadingOverlay = () => {
  return (
    <div className="absolute inset-0 z-20 flex flex-col items-center justify-center bg-black/50 backdrop-blur-[1px]">
      <Loader2 className="h-8 w-8 animate-spin text-white/90" />
      <span className="mt-2 text-xs font-medium tracking-wide text-white/90">Uploading...</span>
    </div>
  )
}

// --- IMAGES ---

type RawImageProps = {
  imageId?: string
  cdnUrl?: string
  aestheticScore: number | undefined | null
}

const RawImage = ({ cdnUrl, aestheticScore, imageId }: RawImageProps) => {
  if (!cdnUrl) return <ProcessingSplash />

  return (
    <>
      {aestheticScore !== undefined && aestheticScore !== null && (
        <div className="absolute top-2 right-2 z-10 flex h-8 w-8 items-center justify-center rounded-full border border-white/20 bg-black/60 backdrop-blur-md">
          <span className="text-xs font-bold text-white">{aestheticScore}</span>
        </div>
      )}
      {/* Select-none is crucial for mobile long-press to work as a context menu 
         instead of the browser trying to select the image 
      */}
      <img
        src={cdnUrl}
        className="h-full w-full object-cover transition-opacity duration-300 select-none"
        alt={`Preview of ${imageId}`}
        loading="lazy"
        draggable={false}
      />
    </>
  )
}

export const RawPreview = memo(function RawPreview({ image, onDelete }: RawPreviewProps) {
  if (!image) return null

  const { cdnUrl, status, aestheticScore, critique, imageId } = image

  const isAnalyzing = status === GalleraiSharedKernelEnumsImageStatus.ANALYZING
  const isUploading = status === GalleraiSharedKernelEnumsImageStatus.UPLOADING

  return (
    <ContextMenu>
      {/* The Trigger wraps the card so the whole area is interactive */}
      <ContextMenuTrigger>
        <ImageCard footer={<AICritiqueSection critique={critique} />}>
          <RawImage imageId={imageId} cdnUrl={cdnUrl} aestheticScore={aestheticScore} />
          {isUploading && <UploadingOverlay />}
          {isAnalyzing && <AnalysisOverlay />}
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
