import { useNavigate } from 'react-router-dom'
import type { Character } from '../types/character'
import NavBar from '../components/NavBar'

const PLACEHOLDER_CHARACTERS: Character[] = [
  { id: 1, playerId: 1, name: 'Aerin', level: 5, setting: 'Scifi', maxHealth: 80 },
  { id: 2, playerId: 1, name: 'Torben', level: 3, setting: 'Unregistered', maxHealth: 60 },
  { id: 3, playerId: 1, name: 'Sylvara', level: 7, setting: 'Scifi', maxHealth: 120 },
]

export default function CharactersPage() {
  const navigate = useNavigate()

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
      <NavBar />

      <div className="max-w-3xl mx-auto px-6 py-8">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-semibold" style={{ color: '#2C2416' }}>Your Characters</h2>
          <button
            onClick={() => navigate('/characters/add')}
            className="rounded-lg px-4 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
            style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
          >
            + Add Character
          </button>
        </div>

        {PLACEHOLDER_CHARACTERS.length === 0 ? (
          <div className="text-center py-20" style={{ color: '#7A6F5C' }}>
            <p className="text-sm">No characters yet. Add one to get started.</p>
          </div>
        ) : (
          <div className="flex flex-col gap-3">
            {PLACEHOLDER_CHARACTERS.map((char) => (
              <div
                key={char.id}
                className="rounded-xl border px-5 py-4 flex items-center justify-between"
                style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
              >
                <div>
                  <p className="font-semibold" style={{ color: '#2C2416' }}>{char.name}</p>
                  <p className="text-sm mt-0.5" style={{ color: '#7A6F5C' }}>{char.setting}</p>
                </div>
                <div className="flex items-center gap-3">
                  <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
                    Level {char.level}
                  </span>
                  <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
                    {char.maxHealth} HP
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}