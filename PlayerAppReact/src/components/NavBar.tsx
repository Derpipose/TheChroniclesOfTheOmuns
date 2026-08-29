import SignOutButton from './SignOutButton'
import { useNavigate } from 'react-router-dom'

interface NavBarProps {
  title?: string
}

export default function NavBar({ title = 'Character Vault' }: NavBarProps) {
  const navigate = useNavigate()

  return (
    <div
      className="border-b px-6 py-4 flex items-center justify-between"
      style={{
        backgroundColor: '#FDFAF4',
        borderColor: '#DDD8CC',
      }}
    >
      <h1
        className="text-lg font-semibold"
        style={{ color: '#2C2416' }}
      >
        {title}
      </h1>

      <div className="flex items-center gap-2">
        <button
          onClick={() => navigate('/rolldice')}
          className="px-3 py-2 text-sm rounded transition-opacity hover:opacity-90"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          Dice Roller
        </button>

        <button
          onClick={() => navigate('/addweapon')}
          className="px-3 py-2 text-sm rounded transition-opacity hover:opacity-90"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          Add Weapon
        </button>

        <button
          onClick={() => navigate('/editweapon')}
          className="px-3 py-2 text-sm rounded transition-opacity hover:opacity-90"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          Edit Weapon
        </button>

        <button
          onClick={() => navigate('/characters')}
          className="px-3 py-2 text-sm rounded transition-opacity hover:opacity-90"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          View Characters
        </button>

        <SignOutButton />
      </div>
    </div>
  )
}
