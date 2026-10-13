import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ProtectedRoute, PublicOnlyRoute } from './auth/ProtectedRoute'
import { ThemeSync } from './theme/ThemeSync.tsx'
import { AuthProvider } from './auth/AuthContext.tsx'
import SignInPage from './pages/SignInPage'
import CharactersPage from './pages/CharactersPage'
import CharacterViewPage from './pages/CharacterViewPage'
import DevPage from './pages/DevPage'
import DiceRollerPage from './pages/DiceRollerPage'
import AddWeaponInstancePage from './pages/AddWeaponInstancePage'
import UpdateWeaponInstancePage from './pages/UpdateWeaponInstancePage'
import PlayerSettingsPage from './pages/PlayerSettings.tsx'
import CharacterFormPage from './pages/CharacterForm.tsx'
import ThemesPage from './pages/ThemesPage.tsx'

const queryClient = new QueryClient()

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <ThemeSync>
            <Routes>
              <Route path="/" element={<Navigate to="/sign-in" replace />} />

            <Route path="/sign-in" element={
              <PublicOnlyRoute>
                <SignInPage />
              </PublicOnlyRoute>
            } />

            <Route path="/characters" element={
              <ProtectedRoute>
                <CharactersPage />
              </ProtectedRoute>
            } />

            <Route path="/characters/add" element={
              <ProtectedRoute>
                <CharacterFormPage mode="add" />

              </ProtectedRoute>
            } />

            <Route path="/character/:id" element={
              <ProtectedRoute>
                <CharacterViewPage />
              </ProtectedRoute>
            } />

            <Route path="/character/:id/edit" element={
              <ProtectedRoute>
                <CharacterFormPage mode="edit" />
              </ProtectedRoute>
            } />

            <Route path="/rolldice" element={
              <ProtectedRoute>
                <DiceRollerPage />
              </ProtectedRoute>
            } />

            <Route path="/addweapon" element={
              <ProtectedRoute>
                <AddWeaponInstancePage />
              </ProtectedRoute>
            } />

            <Route path="/editweapon" element={
              <ProtectedRoute>
                <UpdateWeaponInstancePage />
              </ProtectedRoute>
            } />

            <Route path="/playersettings" element={
              <ProtectedRoute>
                <PlayerSettingsPage />
              </ProtectedRoute>
            } />

            {/* Theme preview sandbox; no auth so it works without the backend */}
            <Route path="/themes" element={<ThemesPage />} />

            {/* Developer Tools and more restricted routes */}

            <Route path="/dev" element={
              <ProtectedRoute>
                <DevPage />
              </ProtectedRoute>
              } />
            </Routes>
          </ThemeSync>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  )
}