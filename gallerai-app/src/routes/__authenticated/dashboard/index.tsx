import { createFileRoute } from '@tanstack/react-router'

import { FolderListSection } from '@/components/gallery/folder-list/folder-list-section'

export const Route = createFileRoute('/__authenticated/dashboard/')({
  component: DashboardComponent,
})

function DashboardComponent() {
  return <FolderListSection />
}
