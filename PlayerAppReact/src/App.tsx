import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import SignInPage from './pages/SignInPage'
import CharactersPage from './pages/CharactersPage'
import AddCharacterPage from './pages/AddCharacterPage'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/sign-in" replace />} />
        <Route path="/sign-in" element={<SignInPage />} />
        <Route path="/characters" element={<CharactersPage />} />
        <Route path="/characters/add" element={<AddCharacterPage />} />
      </Routes>
    </BrowserRouter>
  )
}
