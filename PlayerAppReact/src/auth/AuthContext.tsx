import { createContext, useContext, useState } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { getToken, setToken, clearToken } from './tokenStorage'
import { clearTheme, getTheme } from '../theme/themeStorage'

type AuthContextValue = {
  token: string | null
  isAuthenticated: boolean
  signIn: (token: string) => void
  signOut: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const queryClient = useQueryClient()
  const [token, setTokenState] = useState<string | null>(getToken)

  const signIn = (newToken: string) => {
    setToken(newToken)
    setTokenState(newToken)
  }

  const signOut = () => {
    clearToken()
    clearTheme()
    // Drop cached per-user data so the next sign-in refetches fresh
    queryClient.clear()
    document.documentElement.setAttribute('data-theme', getTheme())
    setTokenState(null)
  }

  return (
    <AuthContext.Provider value={{ token, isAuthenticated: !!token, signIn, signOut }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}