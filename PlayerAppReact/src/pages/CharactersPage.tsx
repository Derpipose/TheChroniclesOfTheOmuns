import { useNavigate } from 'react-router-dom'
import NavBar from '../components/NavBar'
import { useCharacters } from '../hooks/useCharacters'


export default function CharactersPage() {
  const navigate = useNavigate()
  const { data: characters, isLoading, isError } = useCharacters()

  return (
    <div className="min-h-screen" style={{ backgroundColor: 'var(--color-background)' }}>
      <NavBar />
      <div className="max-w-3xl mx-auto px-6 py-8">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-semibold" style={{ color: 'var(--color-text)' }}>Your Characters</h2>
          <button
            onClick={() => navigate('/characters/add')}
            className="rounded-lg px-4 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
            style={{ backgroundColor: 'var(--color-accent)', color: 'var(--color-text)' }}
          >
            Add Character
          </button>
        </div>

        {isLoading && (
          <div className="text-center py-20" style={{ color: 'var(--color-text-60)' }}>
            <p className="text-sm">Loading characters...</p>
          </div>
        )}
        {isError && (
          <div className="text-center py-20" style={{ color: 'var(--color-text-60)' }}>
            <p className="text-sm">Error occurred while fetching characters.</p>
          </div>
        )}
        {!isLoading && !isError && (
          <div className="flex flex-col gap-3">
            {characters && characters.length > 0 ? (
              characters.map((char) => (
                <div
                  key={char.id}
                  className="rounded-xl border px-5 py-4 flex items-center justify-between"
                  style={{ backgroundColor: 'var(--color-background-30)', borderColor: 'var(--color-primary-30)' }}
                  onClick={() => navigate(`/character/${char.id}`)}
                >
                  <div className="flex items-center gap-2">
                    <span
                      className="inline-block w-2 h-2 rounded-full flex-shrink-0"
                      style={{ backgroundColor: char.active ? '#5C8A5C' : 'var(--color-primary-60)' }}
                      title={char.active ? 'Active' : 'In Edit Mode'}
                    />
                    <div>
                      <p className="font-semibold" style={{ color: 'var(--color-text)' }}>{char.name}</p>
                      <p className="text-sm mt-0.5" style={{ color: 'var(--color-text-60)' }}>{char.setting}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <span
                      className="text-xs font-medium px-2.5 py-1 rounded-full"
                      style={{
                        backgroundColor: char.active ? '#E4EEE4' : 'var(--color-accent-30)',
                        color: char.active ? '#3D5C3D' : 'var(--color-primary)',
                      }}
                    >
                      {char.active ? 'Active' : 'In Edit Mode'}
                    </span>
                    <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: 'var(--color-accent-30)', color: 'var(--color-primary)' }}>
                      Level {char.level}
                    </span>
                    <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: 'var(--color-accent-30)', color: 'var(--color-primary)' }}>
                      {char.maxHealth} HP
                    </span>
                  </div>
                </div>
              ))
            ) : (
              <div className="text-center py-20" style={{ color: 'var(--color-text-60)' }}>
                <p className="text-sm">No characters yet. Add one to get started.</p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  )
}