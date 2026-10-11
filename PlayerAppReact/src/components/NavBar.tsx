import SignOutButton from './SignOutButton'
import { useNavigate } from 'react-router-dom'

interface NavBarProps {
  title?: string
}

const NAV_LINKS: { label: string; to: string }[] = [
  { label: 'Dice Roller', to: '/rolldice' },
  { label: 'Add Weapon', to: '/addweapon' },
  { label: 'Edit Weapon', to: '/editweapon' },
  { label: 'View Characters', to: '/characters' },
  { label: 'Settings', to: '/playersettings' },
]

export default function NavBar({ title = 'Character Vault' }: NavBarProps) {
  const navigate = useNavigate()

  return (
    <div className="app-header">
      <h1
        className="text-lg font-semibold shrink-0"
        style={{ color: 'var(--color-text)' }}
      >
        {title}
      </h1>

      <div className="flex flex-1 flex-wrap items-center gap-1.5 pb-1 sm:gap-2 sm:pb-0">
        {NAV_LINKS.map((link) => (
          <button
            key={link.to}
            onClick={() => navigate(link.to)}
            className="nav-btn"
          >
            {link.label}
          </button>
        ))}
      </div>

      <div className="ml-auto shrink-0">
        <SignOutButton />
      </div>
    </div>
  )
}
