export const queryKeys = {
  folders: ['folders'] as const,
  folderImages: (folderId: string) => ['folderImages', folderId] as const,
}
