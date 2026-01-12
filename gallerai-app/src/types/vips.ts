export interface WorkerAPI {
  generatePreview(buffer: ArrayBuffer): Promise<Uint8Array>
}
