import { useNavigate } from 'react-router-dom'
import { useQueryClient } from '@tanstack/react-query'
import { clearToken } from '../auth/tokenStorage'
import { clearTheme, getTheme } from '../theme/themeStorage'

export default function SignOutButton() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const handleSignOut = () => {
    clearToken()
    clearTheme()
    // Drop cached per-user data so the next sign-in refetches and re-syncs the theme
    queryClient.clear()
    document.documentElement.setAttribute('data-theme', getTheme())
    navigate('/sign-in')
  }

  return (
    <button
      onClick={handleSignOut}
      className="nav-btn"
    >
      Sign out
    </button>
  )
}