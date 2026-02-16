import { createContext } from 'react'

export type FolderViewContextType = {
  folderId: string
  setProcessedImageCount: (count: number) => void
  setImageCount: (count: number) => void
}

export const FolderViewContext = createContext<FolderViewContextType>({
  folderId: '',
  setProcessedImageCount: () => {},
  setImageCount: () => {},
})
