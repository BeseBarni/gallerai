import { Card, CardContent, CardHeader } from '@/shadcn/card'
import { Skeleton } from '@/shadcn/skeleton'

export default function FolderListFallback() {
  const skeletonCards = Array.from({ length: 8 }, (_, i) => i)

  return (
    <>
      {skeletonCards.map((index) => (
        <Card key={index} className="border-muted/40 animate-pulse">
          <CardHeader className="flex flex-row items-start justify-between space-y-0 pb-2">
            {/* Folder Icon Placeholder */}
            <Skeleton className="h-8 w-8 rounded-md bg-blue-500/10" />
            {/* Menu Button Placeholder */}
            <Skeleton className="h-8 w-8 rounded-full" />
          </CardHeader>
          <CardContent className="space-y-2">
            {/* Folder Name Placeholder */}
            <Skeleton className="h-5 w-3/4" />
            {/* Item Count Placeholder */}
            <Skeleton className="h-3 w-1/4" />
          </CardContent>
        </Card>
      ))}
    </>
  )
}
