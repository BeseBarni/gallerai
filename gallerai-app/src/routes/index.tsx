import { Button } from '@/shadcn/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shadcn/card'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/')({
  component: RouteComponent,
})

function RouteComponent() {
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

      <Card className="border-primary/20 w-full max-w-md shadow-lg">
        <CardHeader>
          <CardTitle>Library Status</CardTitle>
          <CardDescription>If you see this styled card, shadcn/ui is active.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <p className="text-foreground/80 text-sm">
            Click the button below to test the Tailwind interactive states.
          </p>
          <Button className="w-full" onClick={() => alert('Gallerai is ready!')}>
            Deploy Project
          </Button>
        </CardContent>
      </Card>

      <footer className="text-muted-foreground pt-8 text-sm">
        Thesis Project • Gallerai App 2026
      </footer>
    </div>
  )
}
