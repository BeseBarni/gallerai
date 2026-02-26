import * as Comlink from 'comlink'

import { developRaw } from '@/lib/image-processing/libraw-engine'

const api = {
  async process(buffer: ArrayBuffer) {
    try {
      console.log('Worker received buffer of size:', buffer.byteLength)
      const result = await developRaw(buffer)
      if (result) return Comlink.transfer(result, [result])

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
