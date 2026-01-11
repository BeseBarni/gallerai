import { z } from 'zod'

const envSchema = z.object({
  VITE_API_URL: z.url(),
  MODE: z.enum(['development', 'production', 'test']).default('development'),
})

export const env = envSchema.parse(import.meta.env)
