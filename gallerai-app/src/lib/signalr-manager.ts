import * as signalr from '@microsoft/signalr'

import { env } from './env'

class SignalRManager {
  private static instance: SignalRManager
  private connection: signalr.HubConnection

  private constructor() {
    this.connection = new signalr.HubConnectionBuilder()
      .withUrl(env.VITE_API_HUB_URL)
      .withAutomaticReconnect()
      .build()
  }

  public static getInstance(): SignalRManager {
    if (!SignalRManager.instance) {
      SignalRManager.instance = new SignalRManager()
    }
    return SignalRManager.instance
  }

  private startPromise: Promise<void> | null = null

  public async start() {
    if (this.connection.state === signalr.HubConnectionState.Connected) {
      return
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
}

export const signalRManager = SignalRManager.getInstance()
