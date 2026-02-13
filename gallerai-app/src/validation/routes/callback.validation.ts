import { z } from 'zod'

export const loginCallbackSchema = z.object({
  oneTimeCode: z.string().optional(),
})

export type LoginCallback = z.infer<typeof loginCallbackSchema>
