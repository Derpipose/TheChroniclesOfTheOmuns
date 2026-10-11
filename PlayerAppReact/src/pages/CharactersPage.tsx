import { useNavigate } from 'react-router-dom'
import NavBar from '../components/NavBar'
import { useCharacters } from '../hooks/useCharacters'
import Pill from '../components/Pill'


export default function CharactersPage() {
  const navigate = useNavigate()
  const { data: characters, isLoading, isError } = useCharacters()

  return (
    <div className="page">
      <NavBar />
      <div className="max-w-3xl mx-auto px-6 py-8">
        <div className="flex items-center justify-between mb-6">
          <h2 className="page-title">Your Characters</h2>
          <button
            onClick={() => navigate('/characters/add')}
            className="btn btn-accent px-4"
          >
            Add Character
          </button>
        </div>

        {isLoading && (
          <div className="empty-state">
            <p className="text-sm">Loading characters...</p>
          </div>
        )}
        {isError && (
          <div className="empty-state">
            <p className="text-sm">Error occurred while fetching characters.</p>
          </div>
        )}
        {!isLoading && !isError && (
          <div className="flex flex-col gap-3">
            {characters && characters.length > 0 ? (
              characters.map((char) => (
                <div
                  key={char.id}
                  className="rounded-xl border px-5 py-4 flex items-center justify-between panel"
                  onClick={() => navigate(`/character/${char.id}`)}
                >
                  <div className="flex items-center gap-2">
                    <span
                      className={`status-dot ${char.isActive ? 'status-dot-active' : ''}`}
                      title={char.isActive ? 'Active' : 'In Edit Mode'}
                    />
                    <div>
                      <p className="font-semibold fg">{char.name}</p>
                      <p className="text-sm mt-0.5 fg-muted">{char.setting}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <Pill variant={char.isActive ? 'active' : 'accent'}>{char.isActive ? 'Active' : 'In Edit Mode'}</Pill>
                    <Pill variant="accent">Level {char.level}</Pill>
                    <Pill variant="accent">{char.currentHealth} HP</Pill>
                  </div>
                </div>
              ))
            ) : (
              <div className="empty-state">
                <p className="text-sm">No characters yet. Add one to get started.</p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  )
}
