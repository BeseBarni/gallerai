import { z } from 'zod'

const envSchema = z.object({
  VITE_API_URL: z.string(),
  MODE: z.enum(['development', 'production', 'test']).default('development'),
  VITE_API_HUB_URL: z.string(),
})

export const env = envSchema.parse(import.meta.env)
