import { encodeToJpeg } from '@/utils/image-helpers'
import LibRaw from 'libraw-wasm'

export const developRaw = async (buffer: ArrayBuffer): Promise<ArrayBuffer | null> => {
  // Create the instance once at the start
  const instance = new LibRaw()
  const uint8View = new Uint8Array(buffer)

  try {
    await instance.open(uint8View)

    try {
      const thumb = await instance.thumbnailData()

      if (thumb && thumb.data && thumb.data.length > 50000) {
        console.log(`🚀 Fast extraction successful: ${thumb.width}x${thumb.height}`)

        const cleanBuffer = new Uint8Array(thumb.data).buffer

        return cleanBuffer as ArrayBuffer
      }
    } catch (thumbError) {
      console.warn('Fast extraction failed, falling back to raw development', thumbError)
    }

    console.log('🐢 Processing raw sensor data...')

    await instance.open(uint8View, {
      useCameraWb: true,
      noAutoBright: false,
      halfSize: true, // Speeds up demosaicing significantly
      userQual: 0,
      outputBps: 8,
      medPasses: 0,
      fbddNoiserd: 0,
      highlight: 0,
      outputColor: 1, // sRGB
    })

    const imageData = await instance.imageData()
    const previewBlob = await encodeToJpeg(imageData)

    return await previewBlob.arrayBuffer()
  } catch (error) {
    console.error('WASM Processing failed:', error)
    return null
  }
}
