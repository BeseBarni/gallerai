import ExifReader from 'exifreader'

export const extractPreview = (buffer: ArrayBuffer): Uint8Array | null => {
  try {
    const tags = ExifReader.load(buffer)

    console.log('EXIF Tags:', tags)

    const preview = (tags['PreviewImage'] ||
      tags['JpgFromRaw'] ||
      tags['Thumbnail']) as unknown as { image: ArrayBuffer }

    if (preview && preview.image) {
      const imageBytes = new Uint8Array(preview.image)
      if (imageBytes.length > 500000) {
        return imageBytes
      }
    }
  } catch (error) {
    console.warn('EXIF Extraction failed, falling back to WASM:', error)
  }
  return null
}
