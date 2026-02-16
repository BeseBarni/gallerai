import { Check } from 'lucide-react'

import { cn } from '@/lib/utils'

interface AIProcessCounterProps {
  totalImages: number
  processedImages: number
  size?: number // The overall width/height in px
  strokeWidth?: number // Optional: stroke as a percentage of size (default 8)
  className?: string
}

export const AIProcessCounter = ({
  totalImages,
  processedImages,
  size = 120,
  strokeWidth = 8,
  className,
}: AIProcessCounterProps) => {
  const percentage =
    totalImages === 0 ? 0 : Math.min(100, Math.round((processedImages / totalImages) * 100))
  const isComplete = processedImages === totalImages && totalImages > 0

  // We use a fixed viewBox coordinate system (0-100)
  // so the stroke and circles scale perfectly regardless of the 'size' prop.
  const radius = 50 - strokeWidth / 2
  const circumference = radius * 2 * Math.PI
  const offset = circumference - (percentage / 100) * circumference

  return (
    <div
      className={cn('flex flex-col items-center justify-center gap-[0.5em]', className)}
      style={{
        width: size,
        height: size,
        // This is the magic: setting font-size relative to component size
        fontSize: `${size * 0.15}px`,
      }}
    >
      <div className="relative flex h-full w-full items-center justify-center">
        {/* SVG with ViewBox ensures the circle geometry scales linearly */}
        <svg viewBox="0 0 100 100" className="absolute inset-0 h-full w-full -rotate-90 transform">
          {/* Background Circle */}
          <circle
            cx="50"
            cy="50"
            r={radius}
            stroke="currentColor"
            strokeWidth={strokeWidth}
            fill="transparent"
            className="text-muted/20"
          />
          {/* Progress Circle */}
          <circle
            cx="50"
            cy="50"
            r={radius}
            stroke="currentColor"
            strokeWidth={strokeWidth}
            fill="transparent"
            strokeDasharray={circumference}
            style={{
              strokeDashoffset: offset,
              transition: 'stroke-dashoffset 0.5s ease-in-out',
            }}
            strokeLinecap="round"
            className={cn(
              'transition-all duration-500',
              isComplete ? 'text-green-500' : 'text-primary',
            )}
          />
        </svg>

        {/* Center Content using 'em' units */}
        <div className="z-10 flex flex-col items-center justify-center text-center">
          {isComplete ? (
            <div className="animate-in zoom-in duration-300">
              <Check
                className="text-green-500"
                style={{ width: '2.5em', height: '2.5em', strokeWidth: 3 }}
              />
            </div>
          ) : (
            <>
              <div className="flex items-center justify-center gap-[0.2em]">
                <div
                  className="flex items-baseline justify-center font-bold tracking-tighter tabular-nums"
                  style={{ lineHeight: 1 }} // Force line-height to 1 to prevent vertical "drifting"
                >
                  <span style={{ fontSize: '1.2em' }}>{percentage}</span>
                  <span
                    className="ml-[0.1em] opacity-70"
                    style={{ fontSize: '0.7em' }} // Slightly larger % for better legibility at small sizes
                  >
                    %
                  </span>
                </div>
              </div>
            </>
          )}
        </div>
      </div>

      {/* External Label (Optional) */}
      <span
        className="text-muted-foreground leading-none font-semibold tracking-widest uppercase"
        style={{ fontSize: '0.5em', marginTop: '0.5em' }}
      >
        {isComplete ? 'Complete' : 'AI Processing'}
      </span>
      <div
        className="text-muted-foreground font-medium whitespace-nowrap"
        style={{ fontSize: '0.7em' }}
      >
        {processedImages} / {totalImages}
      </div>
    </div>
  )
}
