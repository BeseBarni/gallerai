import { Suspense } from 'react'
import { queryKeys } from '@/consts/query.keys'
import { useGetFoldersEndpointSuspense } from '@shared/src/api/gallerai/api.gen'

import FolderCard from '../folder/folder-card'
import FolderListFallback from './folder-list-fallback'

function FolderList() {
  const foldersQuery = useGetFoldersEndpointSuspense({ query: { queryKey: queryKeys.folders } })
  const folders = foldersQuery.data?.folders ?? []
  return (
    <>
      {folders.map((folder) => (
        <FolderCard
          key={folder.folderId}
          folder={{
            id: folder.folderId!,
            name: folder.name ?? 'Untitled Folder',
            itemCount: folder.imageCount ?? 0,
          }}
        />
      ))}
    </>
  )
}

export default function FolderListView() {
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Suspense fallback={<FolderListFallback />}>
        <FolderList />
      </Suspense>
    </div>
  )
}
