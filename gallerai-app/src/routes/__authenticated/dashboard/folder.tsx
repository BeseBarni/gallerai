import { useFolderStore } from '@/store/useFolderStore'
import {
  folderViewRouteSchema,
  type FolderViewRoute,
} from '@/validation/routes/folder-view.validation'
import { getFolderByIdEndpoint } from '@shared/src/api/gallerai/api.gen'
import { createFileRoute } from '@tanstack/react-router'

import FolderView from '@/components/gallery/folder-view/folder-view'

export const Route = createFileRoute('/__authenticated/dashboard/folder')({
  validateSearch: (search: FolderViewRoute) => {
    return folderViewRouteSchema.parse(search)
  },
  loaderDeps: ({ search: { folderId } }) => ({ folderId }),
  loader: async ({ deps: { folderId } }) => {
    if (useFolderStore.getState().activeFolder?.id === folderId) {
      return
    }

    const folder = await getFolderByIdEndpoint(folderId).then((p) => p.value)

    if (!folder) return

    useFolderStore.setState({
      activeFolder: {
        id: folder.folderId!,
        name: folder.name ?? 'Untitled Folder',
        itemCount: folder.imageCount ?? 0,
      },
    })
  },
  component: RouteComponent,
})

function RouteComponent() {
  return <FolderView />
}
