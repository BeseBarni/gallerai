import { imageProcessor } from '@/workers/image/worker-pool'
import imageCompression from 'browser-image-compression'
import type { RawImageData } from 'libraw-wasm'

export function toStandardBlob(data: Uint8Array<ArrayBufferLike>, type: string): Blob {
  const copy = new Uint8Array(data.length)
  copy.set(data)
  return new Blob([copy], { type })
}

const rawExtensions = ['.cr2', '.nef', '.arw', '.dng', '.rw2', '.orf', '.raf', '.pef', '.sr2']

export const isRaw = (file: File) =>
  rawExtensions.some((ext) => file.name.toLowerCase().endsWith(ext))

export async function encodeToJpeg(raw: RawImageData): Promise<Blob> {
  const { width, height, data } = raw

  const rgba = new Uint8ClampedArray(width * height * 4)

  for (let i = 0; i < width * height; i++) {
    rgba[i * 4] = data[i * 3] // R
    rgba[i * 4 + 1] = data[i * 3 + 1] // G
    rgba[i * 4 + 2] = data[i * 3 + 2] // B
    rgba[i * 4 + 3] = 255 // A (Opaque)
  }

  const canvas = new OffscreenCanvas(width, height)
  const ctx = canvas.getContext('2d')
  if (!ctx) throw new Error('Could not get canvas context')

  const imgData = new ImageData(rgba, width, height)
  ctx.putImageData(imgData, 0, 0)

  return await canvas.convertToBlob({ type: 'image/jpeg', quality: 0.6 })
}

export const optimizeStandardImage = async (file: File): Promise<Blob> => {
  if (file.size < 2 * 1024 * 1024) return file

  console.log('Optimizing large JPEG...')
  const options = {
    maxSizeMB: 1,
    maxWidthOrHeight: 1920,
    useWebWorker: true,
    fileType: 'image/jpeg',
  }
  return await imageCompression(file, options)
}

export const processImage = async (file: File) => {
  if (isRaw(file)) {
    const buffer = await imageProcessor.process(file)

    if (!buffer) throw new Error('Failed to develop RAW')

    return new Blob([buffer], { type: 'image/jpeg' })
  }

  return await optimizeStandardImage(file)
}
