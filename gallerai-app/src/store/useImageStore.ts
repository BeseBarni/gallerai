import { create } from 'zustand'

interface ProcessingImage {
  id: string
  localUrl: string | null
  status: 'waiting' | 'developing' | 'uploading' | 'ai_processing' | 'done' | 'error'
}

interface ImageStore {
  images: Record<string, ProcessingImage>
  addImage: (image: ProcessingImage) => string
  updateImage: (id: string, updates: Partial<ProcessingImage>) => void
}

export const useImageStore = create<ImageStore>((set) => ({
  images: {},
  addImage: () => {
    const id = crypto.randomUUID()
    set((state) => ({
      images: {
        ...state.images,
        [id]: { id, localUrl: null, status: 'waiting' },
      },
    }))
    return id
  },
  updateImage: (id, updates) =>
    set((state) => ({
      images: { ...state.images, [id]: { ...state.images[id], ...updates } },
    })),
}))
