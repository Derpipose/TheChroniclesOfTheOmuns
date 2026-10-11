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
      className="nav-btn"
    >
      Sign out
    </button>
  )
}