import { ProcessingSplash } from './overlays'

type RawImageProps = {
  imageId?: string
  cdnUrl?: string
  aestheticScore: number | undefined | null
}

export const GalleraiImage = ({ cdnUrl, aestheticScore, imageId }: RawImageProps) => {
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
