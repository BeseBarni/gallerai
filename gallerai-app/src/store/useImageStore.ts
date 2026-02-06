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
      console.log('Updating image', id, state.images, updates, state.images[id], {
        ...state.images[id],
        ...updates,
      })
      return {
        images: { ...state.images, [id]: { ...state.images[id], ...updates } },
      }
    }),
}))
