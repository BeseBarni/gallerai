import { encodeToJpeg } from '@/utils/image-helpers'
import LibRaw from 'libraw-wasm'

export const developRaw = async (buffer: ArrayBuffer): Promise<ArrayBuffer | null> => {
  try {
    const instance = new LibRaw()

    await instance.open(new Uint8Array(buffer), {
      useCameraWb: true,
      noAutoBright: false,
      halfSize: true,
      userQual: 0,
      outputBps: 8,
    })

    const imageData = await instance.imageData()
    const previewBlob = await encodeToJpeg(imageData)

    return await previewBlob.arrayBuffer()
  } catch (error) {
    console.error('WASM Processing failed:', error)
    return null
  }
}
