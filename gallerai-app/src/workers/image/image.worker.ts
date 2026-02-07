import * as Comlink from 'comlink'

import { extractPreview } from '@/lib/image-processing/exif-engine'
import { developRaw } from '@/lib/image-processing/libraw-engine'

const api = {
  async process(buffer: ArrayBuffer) {
    try {
      const fastResult = await extractPreview(buffer)
      if (fastResult && fastResult.byteLength > 500000)
        return Comlink.transfer(fastResult, [fastResult])

      const heavyResult = await developRaw(buffer)
      if (heavyResult) return Comlink.transfer(heavyResult, [heavyResult])

      throw new Error('Unsupported image format')
    } catch (globalWorkerError) {
      console.error('Worker process crashed:', globalWorkerError)
      throw new Error(
        globalWorkerError instanceof Error ? globalWorkerError.message : 'Unknown error',
      )
    }
  },
}

Comlink.expose(api)

export type ImageWorkerAPI = typeof api
