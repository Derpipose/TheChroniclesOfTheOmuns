import type { Character, ScifiCharacter } from '../types/character'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

// --- Base Characters ---

export async function getCharacters(): Promise<Character[]> {
  const res = await fetch(`${BASE_URL}/api/characters`)
  if (!res.ok) throw new Error('Failed to fetch characters')
  return res.json()
}

export async function createCharacter(payload: {
  name: string
  maxHealth: number
}): Promise<Character> {
  const res = await fetch(`${BASE_URL}/api/characters`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error('Failed to create character')
  return res.json()
}

// --- Scifi Characters ---

export async function getScifiCharacters(): Promise<ScifiCharacter[]> {
  const res = await fetch(`${BASE_URL}/api/scifi-characters`)
  if (!res.ok) throw new Error('Failed to fetch scifi characters')
  return res.json()
}

export async function createScifiCharacter(characterId: number): Promise<ScifiCharacter> {
  const res = await fetch(`${BASE_URL}/api/scifi-characters`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ characterId }),
  })
  if (!res.ok) throw new Error('Failed to create scifi character')
  return res.json()
}
