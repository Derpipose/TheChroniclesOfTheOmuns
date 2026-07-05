import { Navigate } from 'react-router-dom'

export function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const token = localStorage.getItem('token')
  if (!token) return <Navigate to="/sign-in" replace />
  return <>{children}</>
}

export function PublicOnlyRoute({ children }: { children: React.ReactNode }) {
  const token = localStorage.getItem('token')
  if (token) return <Navigate to="/characters" replace />
  return <>{children}</>
}