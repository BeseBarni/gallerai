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

  return await canvas.convertToBlob({ type: 'image/jpeg', quality: 0.85 })
}
