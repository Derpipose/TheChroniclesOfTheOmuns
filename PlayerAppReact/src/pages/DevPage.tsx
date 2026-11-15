import NavBar from '../components/NavBar'
import { useDevSeeding } from '../hooks/useDevSeeding'
import AddWeaponCard from '../components/DevPage/AddWeaponCard'
import DeleteWeaponCard from '../components/DevPage/DeleteWeaponCard'
import EditWeaponCard from '../components/DevPage/EditWeaponCard'
import SeedButton from '../components/DevPage/SeedButton'

export default function DevPage() {
  const {
    status,
    messages,
    seedTalents,
    seedClasses,
    seedBackgrounds,
    seedRaces,
    seedSubraces,
    seedWeapons,
  } = useDevSeeding()

  const seeds = [
    { label: '1. Seed Talents', table: 'talents', onClick: seedTalents },
    { label: '2. Seed Classes', table: 'classes', onClick: seedClasses },
    { label: '3. Seed Backgrounds', table: 'backgrounds', onClick: seedBackgrounds },
    { label: '4. Seed Races', table: 'races', onClick: seedRaces },
    { label: '5. Seed Subraces', table: 'subraces', onClick: seedSubraces },
    { label: '6. Seed Weapons', table: 'weapons', onClick: seedWeapons },
  ] as const

  return (
    <div className="page">
      <NavBar title="Dev Tools" />

      <div className="max-w-2xl mx-auto px-6 py-8 flex flex-col gap-6">
        <div>
          <h2 className="page-title">Database Seeding</h2>
          <p className="text-sm mt-1 fg-muted">
            Seed tables in order — talents must be seeded before classes and backgrounds.
          </p>
        </div>

        <div className="flex flex-col gap-3">
          {seeds.map(({ label, table, onClick }) => (
            <SeedButton
              key={table}
              label={label}
              status={status[table]}
              message={messages[table]}
              onClick={onClick}
            />
          ))}
        </div>

        <AddWeaponCard />
        <DeleteWeaponCard />
        <EditWeaponCard />
      </div>
    </div>
  )
}

