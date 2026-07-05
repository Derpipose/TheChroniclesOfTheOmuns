import { useNavigate } from 'react-router-dom'

export default function SignOutButton() {
  const navigate = useNavigate()

  const handleSignOut = () => {
    localStorage.removeItem('token')
    navigate('/sign-in')
  }

  return (
    <button
      onClick={handleSignOut}
      className="text-sm transition-colors hover:underline"
      style={{ color: '#7A6F5C' }}
    >
      Sign out
    </button>
  )
}