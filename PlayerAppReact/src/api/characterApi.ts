
import { apiFetch } from '../auth/apiFetch'
import type { Character } from '../types/character'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export async function getCharacters(): Promise<Character[]> {
  const res = await apiFetch(`${BASE_URL}/api/character`)
  if (!res.ok) throw new Error('Failed to fetch characters')
  return res.json()
}

export async function getCharacterById(id: number): Promise<Character> {
  const res = await apiFetch(`${BASE_URL}/api/character/${id}`)
  if (!res.ok) throw new Error('Failed to fetch character')
  return res.json()
}

export async function createCharacter(payload: {
  name: string
  level: number
}): Promise<Character> {
  const res = await apiFetch(`${BASE_URL}/api/character`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json'},
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error('Failed to create character')
  return res.json()
}

export async function updateCharacter(id: number, payload: Partial<Character>): Promise<Character> {
  const res = await apiFetch(`${BASE_URL}/api/character/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json'},
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error('Failed to update character')
  return res.json()
}
