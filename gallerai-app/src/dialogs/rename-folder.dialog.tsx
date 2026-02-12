import { useState } from 'react'
import { Button } from '@/shadcn/button'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/shadcn/dialog'
import { Input } from '@/shadcn/input'

import type { FolderType } from '@/types/gallery'

export default function RenameFolderDialog({
  folder,
  isOpen,
  onClose,
  onRename,
}: {
  folder: FolderType | null
  isOpen: boolean
  onClose: () => void
  onRename: (id: string, newName: string) => void
}) {
  const [name, setName] = useState(folder?.name || '')

  if (folder && name !== folder.name && !isOpen) setName(folder.name)

  const handleSubmit = () => {
    if (folder && name.trim()) {
      onRename(folder.id, name)
      onClose()
    }
  }

  return (
    <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Rename Folder</DialogTitle>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <Input value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <DialogFooter>
          <Button onClick={handleSubmit}>Save Changes</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
