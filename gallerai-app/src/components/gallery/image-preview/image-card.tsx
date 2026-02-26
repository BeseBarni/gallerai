import type { PropsWithChildren } from 'react'

type ImageCardProps = {
  footer?: React.ReactNode
  badges?: React.ReactNode
} & PropsWithChildren

export const ImageCard = ({ children, footer, badges }: ImageCardProps) => {
  return (
    <div className="bg-card flex flex-col space-y-3 rounded-lg border p-2">
      <div className="relative">
        <div className="bg-muted group relative aspect-square w-full overflow-hidden rounded-md">
          {children}
        </div>
        {badges}
      </div>
      {footer}
    </div>
  )
}
