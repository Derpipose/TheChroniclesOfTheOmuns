import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import SignInPage from './pages/SignInPage'
import CharactersPage from './pages/CharactersPage'
import CharacterViewPage from './pages/CharacterViewPage'
import AddCharacterPage from './pages/AddCharacterPage'
import { ProtectedRoute, PublicOnlyRoute } from './auth/ProtectedRoute'
import DevPage from './pages/DevPage'

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



          <Route path="/dev" element={
            <ProtectedRoute>
              <DevPage />
            </ProtectedRoute>
          } />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>

  )
}