import { useAuthStore } from '@/store/useAuthStore'
import * as signalr from '@microsoft/signalr'

import { env } from '../env'

type HubCallback = (...args: unknown[]) => void

class SignalRManager {
  private static instance: SignalRManager
  private connection: signalr.HubConnection

  private listeners: Map<string, Set<HubCallback>> = new Map()

  private isConnectionAuthenticated: boolean = false

  private constructor() {
    this.connection = new signalr.HubConnectionBuilder()
      .withUrl(env.VITE_API_HUB_URL, {
        accessTokenFactory: () => {
          const token = useAuthStore.getState().token
          this.isConnectionAuthenticated = !!token
          return token || ''
        },
      })
      .withAutomaticReconnect()
      .build()

    this.connection.on('ImageUpdate', (data) => {
      this.emit('ImageUpdate', data)
    })
  }

  public static getInstance(): SignalRManager {
    if (!SignalRManager.instance) {
      SignalRManager.instance = new SignalRManager()
    }
    return SignalRManager.instance
  }

  private startPromise: Promise<void> | null = null

  public async start() {
    const isAuthenticated = useAuthStore.getState().isAuthenticated
    if (this.connection.state === signalr.HubConnectionState.Connected) {
      if (this.isConnectionAuthenticated === isAuthenticated) {
        return
      }
      await this.stop()
    }

    if (this.startPromise) {
      return this.startPromise
    }

    if (this.connection.state === signalr.HubConnectionState.Disconnected) {
      try {
        this.startPromise = this.connection.start()
        await this.startPromise
      } catch (err) {
        this.startPromise = null
        console.error('Error while starting SignalR:', err)
        throw err
      } finally {
        this.startPromise = null
      }
    }
  }

  public async stop() {
    if (this.connection.state !== signalr.HubConnectionState.Disconnected) {
      try {
        await this.connection.stop()
      } catch (err) {
        console.error('Error while stopping SignalR:', err)
      }
    }
  }

  public on(methodName: string, callback: (...args: unknown[]) => void) {
    this.connection.on(methodName, callback)
  }

  public off(methodName: string) {
    this.connection.off(methodName)
  }

  private emit(methodName: string, ...args: unknown[]) {
    this.listeners.get(methodName)?.forEach((callback) => callback(...args))
  }

  public subscribe(methodName: string, callback: HubCallback) {
    if (!this.listeners.has(methodName)) {
      this.listeners.set(methodName, new Set())
    }
    this.listeners.get(methodName)!.add(callback)

    return () => this.listeners.get(methodName)?.delete(callback)
  }
}

export const signalRManager = SignalRManager.getInstance()

export const connectWithRetry = async (attempt: number = 0) => {
  const maxAttempts = env.SIGNALR_RETRY_ATTEMPTS
  const retryDelay = env.SIGNALR_RETRY_DELAY_MS
  try {
    await signalRManager.start()
  } catch (err) {
    if (attempt < maxAttempts) {
      const delay = retryDelay * Math.pow(2, attempt)
      console.warn(
        `SignalR connection failed. Retrying in ${delay}ms... (Attempt ${attempt + 1}/${maxAttempts})`,
      )

      setTimeout(() => connectWithRetry(attempt + 1), delay)
    } else {
      console.error('SignalR connection failed after maximum retries:', err)
    }
  }
}
