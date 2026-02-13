import { FolderListContext, type FolderContextListType } from './context'

export const FolderListProvider = ({
  value,
  children,
}: { value: FolderContextListType } & React.PropsWithChildren) => {
  if (!value) {
    throw new Error('FolderListContext value is required')
  }

  return <FolderListContext.Provider value={value}>{children}</FolderListContext.Provider>
}
