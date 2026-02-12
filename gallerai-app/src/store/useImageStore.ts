import type { GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto } from '@shared/src/api/schemas'
import { create } from 'zustand'

interface ImageStore {
  images: Record<string, GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto>
  addImage: (image: GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto) => string
  updateImage: (
    id: string,
    updates: Partial<GalleraiApplicationFeaturesFoldersGetFolderImagesImageDto>,
  ) => void
}

export const useImageStore = create<ImageStore>((set) => ({
  images: {},
  addImage: ({ imageId, cdnUrl, status }) => {
    set((state) => ({
      images: {
        ...state.images,
        [imageId!]: { imageId, cdnUrl, status },
      },
    }))
    return imageId!
  },
  updateImage: (id, updates) =>
    set((state) => {
      return {
        images: { ...state.images, [id]: { ...state.images[id], ...updates } },
      }
    }),
}))
