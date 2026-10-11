import NavBar from '../components/NavBar'
import Card from '../components/Card'
import { usePlayer, useSetPlayerTheme } from '../hooks/usePlayer'
import { AVAILABLE_THEMES } from '../theme/availableThemes'
import { getTheme } from '../theme/themeStorage'

export default function PlayerSettingsPage() {
  const { data: player, isLoading, isError } = usePlayer()
  const setPlayerTheme = useSetPlayerTheme()

  const activeTheme = player?.theme ?? getTheme()

  return (
    <div className="page">
      <NavBar title="Settings" />
      <div className="max-w-2xl mx-auto px-6 py-8 flex flex-col gap-5">
        <h2 className="page-title">Settings</h2>

        {isLoading && (
          <div className="empty-state">
            <p className="text-sm">Loading your profile...</p>
          </div>
        )}
        {isError && (
          <div className="empty-state">
            <p className="text-sm">Error occurred while loading your profile.</p>
          </div>
        )}

        {!isLoading && !isError && (
          <>
            {/* Profile card */}
            <Card className="flex flex-col gap-4">
              <h3 className="card-title">Profile</h3>
              <div className="flex flex-col gap-3 text-sm">
                <div className="flex items-center justify-between">
                  <span className="fg-muted">Display Name</span>
                  <span className="font-medium fg">{player?.displayName ?? '—'}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="fg-muted">Email</span>
                  <span className="font-medium fg">{player?.email ?? '—'}</span>
                </div>
              </div>
            </Card>

            {/* Theme card */}
            <Card className="flex flex-col gap-4">
              <div className="flex items-center justify-between">
                <h3 className="card-title">Theme</h3>
                {setPlayerTheme.isPending && (
                  <span className="text-xs fg-muted">Saving…</span>
                )}
              </div>
              <p className="text-sm fg-muted">
                Choose the palette used across the app.
              </p>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {AVAILABLE_THEMES.map((theme) => {
                  const isActive = theme.value === activeTheme
                  return (
                    <button
                      key={theme.value}
                      onClick={() => setPlayerTheme.mutate(theme.value)}
                      disabled={setPlayerTheme.isPending}
                      className={`choice-btn ${isActive ? 'choice-btn-active' : ''}`}
                    >
                      <span className="flex items-center justify-between">
                        {theme.label}
                        {isActive && <span className="text-xs">Active</span>}
                      </span>
                    </button>
                  )
                })}
              </div>

              {setPlayerTheme.isError && (
                <p className="text-xs fg-danger">
                  Failed to save theme. Please try again.
                </p>
              )}
            </Card>
          </>
        )}
      </div>
    </div>
  )
}
