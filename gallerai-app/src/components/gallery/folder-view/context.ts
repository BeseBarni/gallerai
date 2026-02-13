import { createContext } from 'react'

export type FolderViewContextType = {
  foldedrId: string
  setProcessedImageCount: (count: number) => void
  setImageCount: (count: number) => void
}

export const FolderViewContext = createContext<FolderViewContextType>({
  foldedrId: '',
  setProcessedImageCount: () => {},
  setImageCount: () => {},
})
