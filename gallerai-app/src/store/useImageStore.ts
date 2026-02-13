import type { GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto } from '@shared/src/api/schemas'
import { create } from 'zustand'

interface ImageStore {
  images: Record<string, GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto>
  addImage: (image: Partial<GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto>) => string
  updateImage: (
    id: string,
    updates: Partial<GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto>,
  ) => void
}

export const useImageStore = create<ImageStore>((set) => ({
  images: {},
  addImage: (image) => {
    set((state) => ({
      images: {
        ...state.images,
        [image.imageId!]: { ...image },
      },
    }))
    return image.imageId!
  },
  updateImage: (id, updates) => {
    set((state) => {
      return {
        images: { ...state.images, [id]: { ...state.images[id], ...updates } },
      }
    })
  },
}))
