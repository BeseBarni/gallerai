import { create } from 'zustand'

export type ImageState = 'waiting' | 'developing' | 'uploading' | 'ai_processing' | 'done' | 'error'

interface ProcessingImage {
  id: string
  localUrl: string | null
  status: ImageState
  score?: number
  critique?: string
}

interface ImageStore {
  images: Record<string, ProcessingImage>
  addImage: (image: ProcessingImage) => string
  updateImage: (id: string, updates: Partial<ProcessingImage>) => void
}

export const useImageStore = create<ImageStore>((set) => ({
  images: {},
  addImage: ({ id, localUrl, status }) => {
    set((state) => ({
      images: {
        ...state.images,
        [id]: { id, localUrl, status },
      },
    }))
    return id
  },
  updateImage: (id, updates) =>
    set((state) => {
      return {
        images: { ...state.images, [id]: { ...state.images[id], ...updates } },
      }
    }),
}))
