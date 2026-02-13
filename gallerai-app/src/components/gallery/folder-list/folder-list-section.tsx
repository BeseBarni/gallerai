import { useState } from 'react'
import { queryKeys } from '@/consts/query.keys'
import CreateFolderDialog from '@/dialogs/create-folder.dialog'
import RenameFolderDialog from '@/dialogs/rename-folder.dialog'
import {
  useAddFolderEndpoint,
  useRemoveFolderEndpoint,
  useRenameFolderEndpoint,
} from '@shared/src/api/gallerai/api.gen'

import type { FolderType } from '@/types/gallery'
import { queryClient } from '@/lib/query-client'

import { FolderListProvider } from '../folder/context/provider'
import FolderListView from './folder-list'

export function FolderListSection() {
  const deleteFolderMutation = useRemoveFolderEndpoint()
  const renameFolderMutation = useRenameFolderEndpoint()
  const createFolderMutation = useAddFolderEndpoint()

  const [renameTarget, setRenameTarget] = useState<FolderType | null>(null)

  const handleCreate = (name: string) => {
    createFolderMutation.mutate(
      { data: { name } },
      {
        onSettled: () => {
          queryClient.invalidateQueries({ queryKey: queryKeys.folders })
        },
      },
    )
  }

  const handleDelete = (id: string) => {
    deleteFolderMutation.mutate(
      { folderId: id },
      {
        onSettled: () => {
          queryClient.invalidateQueries({ queryKey: queryKeys.folders })
        },
      },
    )
  }

  const handleRename = (id: string, newName: string) => {
    renameFolderMutation.mutate(
      { folderId: id, data: { newName } },
      {
        onSettled: () => {
          queryClient.invalidateQueries({ queryKey: queryKeys.folders })
        },
      },
    )
  }
  return (
    <>
      <div className="flex h-full flex-col space-y-8 p-8">
        <div className="flex w-full items-center justify-between space-y-2">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Dashboard</h2>
            <p className="text-muted-foreground">Manage your image folders and uploads.</p>
          </div>
          <CreateFolderDialog onCreate={handleCreate} />
        </div>

        <FolderListProvider
          value={{
            onDelete: handleDelete,
            onRenameStart: setRenameTarget,
          }}
        >
          <FolderListView />
        </FolderListProvider>

        <RenameFolderDialog
          folder={renameTarget}
          isOpen={!!renameTarget}
          onClose={() => setRenameTarget(null)}
          onRename={handleRename}
        />
      </div>
    </>
  )
}
