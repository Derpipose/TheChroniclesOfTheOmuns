import SignOutButton from './SignOutButton'

interface NavBarProps {
  title?: string
}

export default function NavBar({ title = 'Character Vault' }: NavBarProps) {
  return (
    <div className="border-b px-6 py-4 flex items-center justify-between" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
      <h1 className="text-lg font-semibold" style={{ color: '#2C2416' }}>{title}</h1>
      <SignOutButton />
    </div>
  )
}