import { Camera, Sparkles } from 'lucide-react'

export default function GalleraiSplash() {
  return (
    <div className="bg-background text-foreground fixed inset-0 z-50 flex flex-col items-center justify-center transition-colors duration-500">
      {/* Dynamic Ambient Glow - Subtle hues that work in both modes */}
      <div className="pointer-events-none absolute inset-0 overflow-hidden">
        <div className="bg-primary/5 absolute -top-[10%] -left-[10%] h-[40%] w-[40%] rounded-full blur-[120px]" />
        <div className="absolute -right-[10%] -bottom-[10%] h-[40%] w-[40%] rounded-full bg-blue-500/10 blur-[120px]" />
      </div>

      <div className="relative flex flex-col items-center gap-10">
        {/* Animated Logo Container */}
        <div className="group relative">
          {/* AI "Pulse" Ring - Uses primary color opacity */}
          <div className="bg-primary/10 absolute inset-0 animate-ping rounded-full duration-3000" />

          <div className="border-border bg-card relative flex h-28 w-28 items-center justify-center rounded-3xl border shadow-sm backdrop-blur-xl transition-all">
            <Camera className="text-primary h-12 w-12" strokeWidth={1.2} />

            {/* The "AI" Sparkle Badge - High contrast badge */}
            <div className="bg-primary shadow-primary/20 absolute -top-1 -right-1 rounded-full p-1.5 shadow-lg">
              <Sparkles className="text-primary-foreground h-4 w-4" />
            </div>
          </div>
        </div>

        {/* Branding & Typography */}
        <div className="space-y-3 text-center">
          <h1 className="text-4xl font-extralight tracking-[0.25em] uppercase">
            Galler<span className="text-primary font-bold">ai</span>
          </h1>
          <div className="flex items-center justify-center gap-2">
            <span className="bg-border h-px w-4" />
            <p className="text-muted-foreground text-[10px] font-medium tracking-[0.3em] uppercase">
              Processing Vision
            </p>
            <span className="bg-border h-px w-4" />
          </div>
        </div>
      </div>

      {/* Footer Utility Info */}
      <div className="absolute bottom-10 flex flex-col items-center gap-2 opacity-40">
        <p className="text-[9px] font-light tracking-[0.4em] uppercase">
          Photography x Artificial Intelligence
        </p>
      </div>
    </div>
  )
}
