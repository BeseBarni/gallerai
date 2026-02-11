import { z } from 'zod'

const envSchema = z.object({
  VITE_API_URL: z.string(),
  MODE: z.enum(['development', 'production', 'test']).default('development'),
  VITE_API_HUB_URL: z.string(),
  SIGNALR_RETRY_ATTEMPTS: z.number().default(5),
  SIGNALR_RETRY_DELAY_MS: z.number().default(5000),
})

export const env = envSchema.parse(import.meta.env)
