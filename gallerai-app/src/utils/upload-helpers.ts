import axios from 'axios'

interface UploadOptions {
  url: string
  file: Blob | File
  contentType: string
  onProgress: (percent: number) => void
}

export const uploadFileWithProgress = async ({
  url,
  file,
  contentType,
  onProgress,
}: UploadOptions): Promise<void> => {
  await axios.put(url, file, {
    // 1. Override the method to PUT (matches R2 signature)
    headers: {
      'Content-Type': contentType,
      // explicitly clear any global auth headers just in case
      Authorization: undefined,
    },

    // 2. Handle Progress
    onUploadProgress: (progressEvent) => {
      if (progressEvent.total) {
        const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total)
        onProgress(percent)
      }
    },
  })
}
