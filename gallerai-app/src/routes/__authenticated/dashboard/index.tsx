import { useState } from 'react'
import CreateFolderDialog from '@/dialogs/create-folder.dialog'
import RenameFolderDialog from '@/dialogs/rename-folder.dialog'
import {
  useAddFolderEndpoint,
  useGetFoldersEndpoint,
  useRemoveFolderEndpoint,
  useRenameFolderEndpoint,
} from '@shared/src/api/gallerai/api.gen'
import { createFileRoute } from '@tanstack/react-router'

import type { FolderType } from '@/types/gallery'
import FolderCard from '@/components/gallery/folder-card'
import FolderView from '@/components/gallery/folder-view'

export const Route = createFileRoute('/__authenticated/dashboard/')({
  component: DashboardComponent,
})

function DashboardComponent() {
  const folderQuery = useGetFoldersEndpoint()
  const deleteFolderMutation = useRemoveFolderEndpoint()
  const renameFolderMutation = useRenameFolderEndpoint()
  const createFolderMutation = useAddFolderEndpoint()
  const [activeFolderId, setActiveFolderId] = useState<string | null>(null)

  const [renameTarget, setRenameTarget] = useState<FolderType | null>(null)

  const handleCreate = (name: string) => {
    createFolderMutation.mutate(
      { data: { name } },
      {
        onSettled: () => {
          folderQuery.refetch()
        },
      },
    )
  }

  const handleDelete = (id: string) => {
    deleteFolderMutation.mutate(
      { folderId: id },
      {
        onSettled: () => {
          folderQuery.refetch()
        },
      },
    )
  }

  const handleRename = (id: string, newName: string) => {
    renameFolderMutation.mutate(
      { folderId: id, data: { newName } },
      {
        onSettled: () => {
          folderQuery.refetch()
        },
      },
    )
  }

  if (folderQuery.isLoading) {
    return <div>Loading your folders...</div>
  }

  const activeFolder = folderQuery.data?.value?.folders?.find((f) => f.folderId === activeFolderId)

  if (activeFolder && activeFolder.folderId) {
    return (
      <FolderView
        folderId={activeFolder.folderId}
        setActiveFolderId={setActiveFolderId}
        name={activeFolder.name ?? ''}
      />
    )
  }

  return (
    <div className="flex h-full flex-col space-y-8 p-8">
      <div className="flex w-full items-center justify-between space-y-2">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Dashboard</h2>
          <p className="text-muted-foreground">Manage your image folders and uploads.</p>
        </div>
        <CreateFolderDialog onCreate={handleCreate} />
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {folderQuery.data?.value?.folders
          ?.filter((p) => p.folderId)
          .map((folder) => (
            <FolderCard
              key={folder.folderId}
              folder={{
                id: folder.folderId!,
                name: folder.name ?? 'Untitled Folder',
                itemCount: 0,
              }}
              onOpen={setActiveFolderId}
              onDelete={handleDelete}
              onRenameStart={setRenameTarget}
            />
          ))}
      </div>

      <RenameFolderDialog
        folder={renameTarget}
        isOpen={!!renameTarget}
        onClose={() => setRenameTarget(null)}
        onRename={handleRename}
      />
    </div>
  )
}
