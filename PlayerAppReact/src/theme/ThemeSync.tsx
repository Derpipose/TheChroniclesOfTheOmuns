import { usePlayer } from '../hooks/usePlayer'
import { setTheme, getTheme } from './themeStorage'
import { useEffect } from 'react'

// theme that wraps children with the theme from localStorage
export function ThemeSync({ children }: { children: React.ReactNode }) {
  const { data: player } = usePlayer()

  document.documentElement.setAttribute('data-theme', getTheme())

  useEffect(() => {
    console.log("player", player)
    if (player) {
      setTheme(player.theme)
    }
  }, [player])

  return <>{children}</>
}