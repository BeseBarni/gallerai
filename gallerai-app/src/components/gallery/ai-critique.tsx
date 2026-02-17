import { useEffect, useState } from 'react'

// --- Typewriter Hook ---
const useTypewriter = (text: string, speed: number = 15) => {
  const [displayedText, setDisplayedText] = useState('')

  useEffect(() => {
    let i = 0
    setDisplayedText('') // Reset when text changes

    const timer = setInterval(() => {
      setDisplayedText(text.slice(0, i))
      i++

      if (i > text.length) clearInterval(timer)
    }, speed)

    return () => clearInterval(timer)
  }, [text, speed])

  return displayedText
}

// --- Critique Section ---
export const AICritique = ({ critique }: { critique?: string }) => {
  const animatedText = useTypewriter(critique || '', 10) // Quick speed

  if (!critique) return null

  return (
    <div className="space-y-1 px-1 pb-1">
      <h4 className="text-muted-foreground text-[10px] font-bold tracking-wider uppercase">
        AI Critique
      </h4>
      <p className="text-foreground/80 text-sm leading-relaxed italic">"{animatedText}"</p>
    </div>
  )
}
