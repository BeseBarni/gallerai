import { env } from '@/lib/env'

export const isDevMode = () => env.MODE === 'development'
export const isProdMode = () => env.MODE === 'production'
