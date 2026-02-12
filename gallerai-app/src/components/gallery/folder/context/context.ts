import { createContext } from 'react'

import type { FolderType } from '@/types/gallery'

export type FolderContextListType = {
  onDelete: (id: string) => void
  onRenameStart: (folder: FolderType) => void
}
export const FolderListContext = createContext<FolderContextListType | null>(null)
