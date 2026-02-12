import { Button } from '@/shadcn/button'
import { useAuthStore } from '@/store/useAuthStore'
import { useHelloEndpoint } from '@gallerai/shared/web'
import { createFileRoute, Link } from '@tanstack/react-router'

export const Route = createFileRoute('/')({
  component: RouteComponent,
})

function RouteComponent() {
  const { data } = useHelloEndpoint()
  const { isAuthenticated, email, logout } = useAuthStore()

  return (
    <div className="bg-background flex min-h-screen flex-col items-center justify-center space-y-8 p-6">
      <div className="max-w-2xl space-y-4 text-center">
        <h1 className="text-foreground text-5xl font-extrabold tracking-tight lg:text-6xl">
          Welcome to <span className="text-primary">Gallerai</span>
        </h1>
        <p className="text-muted-foreground text-xl">
          The future of AI-powered galleries. Built for speed, styled for impact.
        </p>
      </div>
      {isAuthenticated ? (
        <div className="flex items-center gap-4">
          <span className="text-muted-foreground text-sm">{email}</span>
          <Button variant="outline" size="sm" onClick={logout}>
            Logout
          </Button>
        </div>
      ) : (
        <Link to="/login">
          <Button variant="outline" size="sm">
            Login
          </Button>
        </Link>
      )}
      <footer className="text-muted-foreground pt-8 text-sm">
        Thesis Project • Gallerai App 2026
        <br />
        <p className="text-center">{data?.message ?? 'Hello endpoint is not working.'}</p>
      </footer>
    </div>
  )
}
