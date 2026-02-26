import * as Comlink from 'comlink'

import type { ImageWorkerAPI } from './image.worker'
import ImageWorker from './image.worker.ts?worker'

type ImageQueueItem = {
  file: File
  resolve: (value: ArrayBuffer) => void
  reject: (reason?: unknown) => void
}

class ImageProcessorPool {
  private workers: { api: Comlink.Remote<ImageWorkerAPI>; busy: boolean }[] = []
  private queue: ImageQueueItem[] = []

  private readonly MAX_CONCURRENT = navigator.hardwareConcurrency - 1

  constructor() {}

  public async process(file: File): Promise<ArrayBuffer> {
    return new Promise((resolve, reject) => {
      this.queue.push({ file, resolve, reject })
      this.next()
    })
  }

  private getFreeWorker() {
    const idleWorker = this.workers.find((w) => !w.busy)
    if (idleWorker) return idleWorker

    if (this.workers.length < this.MAX_CONCURRENT) {
      const newWorkerInstance = new ImageWorker()

      const newWorker = {
        api: Comlink.wrap<ImageWorkerAPI>(newWorkerInstance),
        busy: false,
      }

      this.workers.push(newWorker)
      return newWorker
    }

    return null
  }

  private async next() {
    if (this.queue.length === 0) return

    const worker = this.getFreeWorker()

    if (!worker) return

    const task = this.queue.shift()
    if (!task) return

    worker.busy = true

    try {
      const buffer = await task.file.arrayBuffer()
      console.log('Main thread read file buffer of size:', buffer.byteLength)
      const result = await worker.api.process(Comlink.transfer(buffer, [buffer]))
      task.resolve(result)
    } catch (err) {
      task.reject(err)
    } finally {
      worker.busy = false
      this.next()
    }
  }
}

export const imageProcessor = new ImageProcessorPool()
