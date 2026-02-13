import z from 'zod'

export const folderViewRouteSchema = z.object({
  folderId: z.string(),
})

export type FolderViewRoute = z.infer<typeof folderViewRouteSchema>
