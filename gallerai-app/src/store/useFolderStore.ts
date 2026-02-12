import { create } from 'zustand'
import { persist } from 'zustand/middleware'

import type { FolderType } from '@/types/gallery'

interface FolderStore {
  activeFolder: FolderType | null
  setActiveFolder: (folder: FolderType | null) => void
}

export const useFolderStore = create<FolderStore>()(
  persist(
    (set) => ({
      activeFolder: null,
      setActiveFolder: (folder) => set({ activeFolder: folder }),
    }),
    {
      name: 'folder-storage',
    },
  ),
)
