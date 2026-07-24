import NavBar from '../components/NavBar'
import { useDevSeeding } from '../hooks/useDevSeeding'
import type { SeedResult, SeedStatus } from '../types/devSeed'

const statusColor = (s: SeedStatus) => {
  if (s === 'success') return '#4A7C59'
  if (s === 'error') return '#B04A2F'
  if (s === 'loading') return '#8B7355'
  return '#7A6F5C'
}

const statusLabel = (s: SeedStatus) => {
  if (s === 'success') return '✓ Done'
  if (s === 'error') return '✗ Failed'
  if (s === 'loading') return 'Loading...'
  return 'Not run'
}

export default function DevPage() {
  const {
    status,
    messages,
    seedTalents,
    seedClasses,
    seedBackgrounds,
    seedRaces,
    seedSubraces,
  } = useDevSeeding()

  const SeedButton = ({ label, onClick, table }: {
    label: string
    onClick: () => void
    table: keyof SeedResult
  }) => (
    <div
      className="rounded-xl border px-5 py-4 flex flex-col gap-1"
      style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
    >
      <div className="flex items-center justify-between">
        <div>
          <p className="font-semibold" style={{ color: '#2C2416' }}>{label}</p>
          <p className="text-xs mt-0.5" style={{ color: statusColor(status[table]) }}>
            {statusLabel(status[table])}
          </p>
        </div>
        <button
          onClick={onClick}
          disabled={status[table] === 'loading'}
          className="rounded-lg px-4 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50"
          style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
        >
          Seed
        </button>
      </div>
      {messages[table] && (
        <p className="text-xs break-words" style={{ color: '#7A6F5C' }}>
          {messages[table]}
        </p>
      )}
    </div>
  )

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
      <NavBar title="Dev Tools" />

      <div className="max-w-2xl mx-auto px-6 py-8">
        <div className="mb-6">
          <h2 className="text-xl font-semibold" style={{ color: '#2C2416' }}>Database Seeding</h2>
          <p className="text-sm mt-1" style={{ color: '#7A6F5C' }}>
            Seed tables in order — talents must be seeded before classes and backgrounds.
          </p>
        </div>

        <div className="flex flex-col gap-3">
          <SeedButton label="1. Seed Talents" onClick={seedTalents} table="talents" />
          <SeedButton label="2. Seed Classes" onClick={seedClasses} table="classes" />
          <SeedButton label="3. Seed Backgrounds" onClick={seedBackgrounds} table="backgrounds" />
          <SeedButton label="4. Seed Races" onClick={seedRaces} table="races" />
          <SeedButton label="5. Seed Subraces" onClick={seedSubraces} table="subraces" />
        </div>
      </div>
    </div>
  )
}