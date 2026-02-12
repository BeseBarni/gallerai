import { useEffect } from 'react'
import { useAuthStore } from '@/store/useAuthStore'
import { loginCallbackSchema, type LoginCallback } from '@/validation/routes/callback.validation'
import { acquireTokenEndpoint } from '@shared/src/api/gallerai/api.gen'
import { createFileRoute, useNavigate } from '@tanstack/react-router'

export const Route = createFileRoute('/auth/callback')({
  validateSearch: (search): LoginCallback => {
    return loginCallbackSchema.parse(search)
  },
  loaderDeps: ({ search: { oneTimeCode } }) => ({ oneTimeCode }),
  loader: async ({ deps: { oneTimeCode } }) => {
    console.log('Received OTP:', oneTimeCode)
    const response = await acquireTokenEndpoint({
      oneTimeCode: oneTimeCode,
    })

    if (!response.isSuccess || !response.value?.token) {
      throw new Error('Failed to acquire token')
    }

    return response.value.token
  },
  pendingComponent: () => <div>Verifying your code...</div>,
  errorComponent: () => <div>Authentication failed. Please try logging in again.</div>,
  component: AuthCallbackPage,
})

function AuthCallbackPage() {
  const token = Route.useLoaderData()

  const { setAuth } = useAuthStore()

  const navigate = useNavigate()

  useEffect(() => {
    setAuth(token, '', '')
    navigate({ to: '/' })
  }, [token, setAuth, navigate])

  return (
    <div className="bg-background flex min-h-screen flex-col items-center justify-center p-6">
      <div className="text-center">
        <h1 className="text-foreground text-2xl font-bold">Authenticating...</h1>
        <p className="text-muted-foreground mt-2">Please wait while we sign you in.</p>
      </div>
    </div>
  )
}
