import { useNavigate } from 'react-router-dom'
import { clearToken } from '../auth/tokenStorage'
import { clearTheme } from '../theme/themeStorage'

export default function SignOutButton() {
  const navigate = useNavigate()

  const handleSignOut = () => {
    clearToken()
    clearTheme()
    navigate('/sign-in')
  }

  return (
    <button
      onClick={handleSignOut}
      className="nav-btn shrink-0 whitespace-nowrap px-2 py-1.5 text-xs rounded sm:px-3 sm:py-2 sm:text-sm"
    >
      Sign out
    </button>
  )
}