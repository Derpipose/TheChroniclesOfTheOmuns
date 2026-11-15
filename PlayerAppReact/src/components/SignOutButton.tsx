import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export default function SignOutButton() {
  const navigate = useNavigate()
  const { signOut } = useAuth()

  const handleSignOut = () => {
    signOut()
    navigate('/sign-in')
  }

  return (
    <button onClick={handleSignOut} className="nav-btn">
      Sign out
    </button>
  )
}