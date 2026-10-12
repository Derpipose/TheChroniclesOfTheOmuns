import { useLocation } from 'react-router-dom'
import { usePlayer } from '../hooks/usePlayer'
import { setTheme } from './themeStorage'
import { useEffect } from 'react'

export function ThemeSync({ children }: { children: React.ReactNode }) {
  // Re-render on navigation so usePlayer's `enabled: !!getToken()` is re-evaluated after sign-in/out
  useLocation()
  const { data: player } = usePlayer()

  useEffect(() => {
    if (player) {
      setTheme(player.theme)
      document.documentElement.setAttribute('data-theme', player.theme)
    }
  }, [player])

  return <>{children}</>
}
