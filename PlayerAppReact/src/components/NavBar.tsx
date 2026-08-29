import SignOutButton from './SignOutButton'
import { useNavigate } from 'react-router-dom'

interface NavBarProps {
  title?: string
}

export default function NavBar({ title = 'Character Vault' }: NavBarProps) {
  const navigate = useNavigate()

  return (
    <div
      className="border-b px-4 py-3 sm:px-6 sm:py-4 flex flex-wrap items-center gap-3"
      style={{
        backgroundColor: '#FDFAF4',
        borderColor: '#DDD8CC',
      }}
    >
      <h1
        className="text-lg font-semibold shrink-0"
        style={{ color: '#2C2416' }}
      >
        {title}
      </h1>

      <div className="flex w-full flex-wrap items-center gap-1.5 pb-1 sm:w-auto sm:gap-2 sm:pb-0">
        <button
          onClick={() => navigate('/rolldice')}
          className="shrink-0 whitespace-nowrap px-2 py-1.5 text-xs rounded transition-opacity hover:opacity-90 sm:px-3 sm:py-2 sm:text-sm"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          Dice Roller
        </button>

        <button
          onClick={() => navigate('/addweapon')}
          className="shrink-0 whitespace-nowrap px-2 py-1.5 text-xs rounded transition-opacity hover:opacity-90 sm:px-3 sm:py-2 sm:text-sm"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          Add Weapon
        </button>

        <button
          onClick={() => navigate('/editweapon')}
          className="shrink-0 whitespace-nowrap px-2 py-1.5 text-xs rounded transition-opacity hover:opacity-90 sm:px-3 sm:py-2 sm:text-sm"
          style={{
            backgroundColor: '#EDE8DC',
            color: '#5C4F38',
          }}
        >
          Edit Weapon
        </button>

        <button
          onClick={() => navigate('/characters')}
          className="shrink-0 whitespace-nowrap px-2 py-1.5 text-xs rounded transition-opacity hover:opacity-90 sm:px-3 sm:py-2 sm:text-sm"
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
