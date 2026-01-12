import * as Comlink from 'comlink'

import type { ImageWorkerAPI } from './image.worker'

type ImageQueueItem = {
  file: File
  resolve: (value: Uint8Array) => void
  reject: (reason?: unknown) => void
}

class ImageProcessorPool {
  private workers: Comlink.Remote<ImageWorkerAPI>[] = []
  private queue: ImageQueueItem[] = []

  private activeTasks = 0

  private readonly MAX_CONCURRENT = Math.min(navigator.hardwareConcurrency || 4, 3)

  constructor() {
    for (let i = 0; i < this.MAX_CONCURRENT; i++) {
      const worker = new Worker(new URL('./image.worker.ts', import.meta.url), {
        type: 'module',
      })
      this.workers.push(Comlink.wrap<ImageWorkerAPI>(worker))
    }
  }

  public async process(file: File): Promise<Uint8Array> {
    return new Promise((resolve, reject) => {
      this.queue.push({ file, resolve, reject })
      this.next()
    })
  }

  private async next() {
    if (this.activeTasks >= this.MAX_CONCURRENT || this.queue.length === 0) return

    const task = this.queue.shift()
    if (!task) return

    this.activeTasks++

    const workerIndex = this.activeTasks % this.workers.length
    const worker = this.workers[workerIndex]

    try {
      const buffer = await task.file.arrayBuffer()

      const result = await worker.process(Comlink.transfer(buffer, [buffer]))

      task.resolve(result)
    } catch (err) {
      task.reject(err)
    } finally {
      this.activeTasks--
      this.next()
    }
  }
}

export const imageProcessor = new ImageProcessorPool()
