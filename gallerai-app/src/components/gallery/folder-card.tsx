import { Button } from '@/shadcn/button'
import { Card, CardContent, CardHeader } from '@/shadcn/card'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/shadcn/dropdown-menu'
import { Edit2, Folder, MoreVertical, Trash2 } from 'lucide-react'

import type { FolderType } from '@/types/gallery'

type FolderCardProps = {
  folder: FolderType
  onOpen: (id: string) => void
  onDelete: (id: string) => void
  onRenameStart: (folder: FolderType) => void
}

export default function FolderCard({ folder, onOpen, onDelete, onRenameStart }: FolderCardProps) {
  return (
    <Card
      className="group hover:border-primary/50 cursor-pointer transition-all"
      onClick={() => onOpen(folder.id)}
    >
      <CardHeader className="flex flex-row items-start justify-between space-y-0 pb-2">
        <Folder className="h-8 w-8 fill-blue-500/20 text-blue-500" />
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              variant="ghost"
              className="h-8 w-8 p-0 opacity-0 transition-opacity group-hover:opacity-100"
            >
              <span className="sr-only">Open menu</span>
              <MoreVertical className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" onClick={(e) => e.stopPropagation()}>
            <DropdownMenuItem onClick={() => onRenameStart(folder)}>
              <Edit2 className="mr-2 h-4 w-4" /> Rename
            </DropdownMenuItem>
            <DropdownMenuItem
              className="text-red-600 focus:text-red-600"
              onClick={() => onDelete(folder.id)}
            >
              <Trash2 className="mr-2 h-4 w-4" /> Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </CardHeader>
      <CardContent>
        <div className="truncate text-lg font-bold">{folder.name}</div>
        <p className="text-muted-foreground text-xs">{folder.itemCount} items</p>
      </CardContent>
    </Card>
  )
}
