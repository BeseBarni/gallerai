import { create } from 'zustand'

interface ProcessingImage {
  id: string
  file: File
  localUrl: string | null
  cdnUrl: string | null
  status: 'waiting' | 'developing' | 'uploading' | 'ai_processing' | 'done' | 'error'
}

interface ImageStore {
  images: Record<string, ProcessingImage>
  addImage: (file: File) => string
  updateImage: (id: string, updates: Partial<ProcessingImage>) => void
}

export const useImageStore = create<ImageStore>((set) => ({
  images: {},
  addImage: (file) => {
    const id = crypto.randomUUID()
    set((state) => ({
      images: {
        ...state.images,
        [id]: { id, file, localUrl: null, cdnUrl: null, status: 'waiting' },
      },
    }))
    return id
  },
  updateImage: (id, updates) =>
    set((state) => ({
      images: { ...state.images, [id]: { ...state.images[id], ...updates } },
    })),
}))
