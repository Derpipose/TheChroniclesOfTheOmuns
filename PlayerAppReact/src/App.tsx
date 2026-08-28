import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import SignInPage from './pages/SignInPage'
import CharactersPage from './pages/CharactersPage'
import CharacterViewPage from './pages/CharacterViewPage'
import AddCharacterPage from './pages/AddCharacterPage'
import EditCharacterPage from './pages/EditCharacterPage.tsx'
import { ProtectedRoute, PublicOnlyRoute } from './auth/ProtectedRoute'
import DevPage from './pages/DevPage'
import DiceRollerPage from './pages/DiceRollerPage'
import AddWeaponInstancePage from './pages/AddWeaponInstancePage'
import UpdateWeaponInstancePage from './pages/UpdateWeaponInstancePage'

const queryClient = new QueryClient()

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
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
              <AddCharacterPage />
            </ProtectedRoute>
          } />
          <Route path="/character/:id" element={
            <ProtectedRoute>
              <CharacterViewPage />
            </ProtectedRoute>
          } />
          <Route path="/character/:id/edit" element={
            <ProtectedRoute>
              <EditCharacterPage />
            </ProtectedRoute>
          } />



          <Route path="/dev" element={
            <ProtectedRoute>
              <DevPage />
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
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>

  )
}
