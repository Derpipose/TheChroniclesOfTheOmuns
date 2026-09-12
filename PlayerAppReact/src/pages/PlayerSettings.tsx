import NavBar from '../components/NavBar'
import { usePlayer, useSetPlayerTheme } from '../hooks/usePlayer'
import { AVAILABLE_THEMES } from '../theme/availableThemes'
import { getTheme } from '../theme/themeStorage'

export default function PlayerSettingsPage() {
  const { data: player, isLoading, isError } = usePlayer()
  const setPlayerTheme = useSetPlayerTheme()

  const activeTheme = player?.theme ?? getTheme()

  return (
    <div className="min-h-screen" style={{ backgroundColor: 'var(--color-background)' }}>
      <NavBar />
      <div className="max-w-2xl mx-auto px-6 py-8 flex flex-col gap-5">
        <h2 className="text-xl font-semibold" style={{ color: 'var(--color-text)' }}>Settings</h2>

        {isLoading && (
          <div className="text-center py-20" style={{ color: 'var(--color-text-60)' }}>
            <p className="text-sm">Loading your profile...</p>
          </div>
        )}
        {isError && (
          <div className="text-center py-20" style={{ color: 'var(--color-text-60)' }}>
            <p className="text-sm">Error occurred while loading your profile.</p>
          </div>
        )}

        {!isLoading && !isError && (
          <>
            {/* Profile card */}
            <div
              className="rounded-2xl border p-6 flex flex-col gap-4"
              style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}
            >
              <h3 className="text-sm font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Profile</h3>
              <div className="flex flex-col gap-3 text-sm">
                <div className="flex items-center justify-between">
                  <span style={{ color: 'var(--color-text-60)' }}>Display Name</span>
                  <span className="font-medium" style={{ color: 'var(--color-text)' }}>{player?.displayName ?? '—'}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span style={{ color: 'var(--color-text-60)' }}>Email</span>
                  <span className="font-medium" style={{ color: 'var(--color-text)' }}>{player?.email ?? '—'}</span>
                </div>
              </div>
            </div>

            {/* Theme card */}
            <div
              className="rounded-2xl border p-6 flex flex-col gap-4"
              style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}
            >
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Theme</h3>
                {setPlayerTheme.isPending && (
                  <span className="text-xs" style={{ color: 'var(--color-text-60)' }}>Saving…</span>
                )}
              </div>
              <p className="text-sm" style={{ color: 'var(--color-text-60)' }}>
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
                      className="rounded-xl border px-4 py-3 text-left text-sm font-medium transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50"
                      style={{
                        backgroundColor: isActive ? 'var(--color-accent-30)' : 'var(--color-background-30)',
                        borderColor: isActive ? 'var(--color-accent)' : 'var(--color-primary-30)',
                        color: isActive ? 'var(--color-primary)' : 'var(--color-text)',
                      }}
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
                <p className="text-xs" style={{ color: 'var(--color-secondary)' }}>
                  Failed to save theme. Please try again.
                </p>
              )}
            </div>
          </>
        )}
      </div>
    </div>
  )
}
