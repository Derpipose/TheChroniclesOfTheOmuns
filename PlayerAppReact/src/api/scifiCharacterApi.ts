// --- Scifi Characters ---

import type { CreateScifiCharacter, ScifiCharacter, UpdateScifiCharacter } from "../types/character"
import { apiFetch } from "../auth/apiFetch"

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export async function getScifiCharacterById(id: number): Promise<ScifiCharacter> {
  const response = await apiFetch(`${BASE_URL}/api/scifi/scifiCharacter/${id}`)

  if (!response.ok) {
    throw new Error(`Failed to fetch scifi character with id ${id}: ${response.statusText}`)
  }

  return response.json()
}

export async function getScifiCharacterByCharacterId(characterId: number): Promise<ScifiCharacter> {
  const response = await apiFetch(`${BASE_URL}/api/scifi/scificharacter/by-character/${characterId}`)

  if (!response.ok) {
    throw new Error(`Failed to fetch scifi character for character ${characterId}: ${response.statusText}`)
  }

  return response.json()
}

export async function createScifiCharacter(newScifiCharacter: CreateScifiCharacter): Promise<ScifiCharacter> {
  const response = await apiFetch(`${BASE_URL}/api/scifi/scifiCharacter`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(newScifiCharacter),
  })

  if (!response.ok) {
    throw new Error(`Failed to create scifi character: ${response.statusText}`)
  }

  return response.json()
}

export async function updateScifiCharacter(id: number, updateScifiCharacter: UpdateScifiCharacter): Promise<ScifiCharacter> {
  const response = await apiFetch(`${BASE_URL}/api/scifi/scifiCharacter/${id}`, {
    method: 'PATCH',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(updateScifiCharacter),
  })

  if (!response.ok) {
    throw new Error(`Failed to update scifi character with id ${id}: ${response.statusText}`)
  }

  return response.json()
}

export async function deleteScifiCharacter(id: number): Promise<void> {
  const response = await apiFetch(`${BASE_URL}/api/scifi/scifiCharacter/${id}`, {
    method: 'DELETE',
  })

  if (!response.ok) {
    throw new Error(`Failed to delete scifi character with id ${id}: ${response.statusText}`)
  }
}
