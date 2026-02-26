import { AlertCircle, Loader2, RefreshCcw, Sparkles } from 'lucide-react'

export const ProcessingSplash = () => {
  return (
    <div className="flex h-full animate-pulse flex-col items-center justify-center space-y-1">
      <span className="text-muted-foreground text-xs font-medium">{'Queued'}</span>
    </div>
  )
}

export const AnalysisOverlay = () => {
  return (
    <div className="absolute inset-0 z-20 flex flex-col items-center justify-center bg-black/40 backdrop-blur-[2px] transition-all duration-500">
      <div className="relative">
        <Sparkles className="text-primary h-8 w-8 animate-pulse drop-shadow-[0_0_8px_rgba(253,224,71,0.6)]" />
        <Sparkles className="absolute -top-2 -right-2 h-4 w-4 animate-bounce text-white opacity-80 duration-1000" />
      </div>
      <span className="mt-2 animate-pulse text-xs font-medium tracking-wide text-white/90">
        Analyzing...
      </span>
    </div>
  )
}

export const UploadingOverlay = () => {
  return (
    <div className="absolute inset-0 z-20 flex flex-col items-center justify-center bg-black/50 backdrop-blur-[1px]">
      <Loader2 className="h-8 w-8 animate-spin text-white/90" />
      <span className="mt-2 text-xs font-medium tracking-wide text-white/90">Uploading...</span>
    </div>
  )
}

export const ErrorOverlay = ({ onRetry }: { onRetry: () => void }) => {
  return (
    <div className="absolute inset-0 z-20 flex flex-col items-center justify-center bg-black/60 p-4 text-center backdrop-blur-[1px]">
      <AlertCircle className="mb-2 h-8 w-8 text-red-400" />
      <span className="mb-3 text-xs font-medium text-white/90">Analysis Failed</span>
      <button
        onClick={(e) => {
          e.stopPropagation() // Prevent triggering parent clicks/context menus
          onRetry()
        }}
        className="group flex items-center gap-1.5 rounded-md bg-white/10 px-3 py-1.5 text-xs font-medium text-white transition-colors hover:bg-white/20 active:bg-white/30"
      >
        <RefreshCcw className="h-3 w-3 transition-transform group-hover:rotate-180" />
        Retry
      </button>
    </div>
  )
}
