import ExifReader from 'exifreader'

export const extractPreview = (buffer: ArrayBuffer): ArrayBuffer | null => {
  try {
    const tags = ExifReader.load(buffer)

    console.log('EXIF Tags:', tags)

    const preview = (tags['PreviewImage'] ||
      tags['JpgFromRaw'] ||
      tags['Thumbnail']) as unknown as { image: ArrayBuffer }

    if (!preview || !preview.image) return null

    if (preview.image.byteLength < 500000) return null

    return preview.image
  } catch (error) {
    console.warn('EXIF Extraction failed, falling back to WASM:', error)
  }
  return null
}
