import { Skeleton } from '@/shadcn/skeleton'
import { useFolderStore } from '@/store/useFolderStore'

export default function FolderImagesFallback() {
  const imageCount = useFolderStore((state) => state.activeFolder!.itemCount)
  return (
    <>
      {[...Array(imageCount)].map((_, index) => (
        <Skeleton key={index} className="aspect-square h-full" />
      ))}
    </>
  )
}
