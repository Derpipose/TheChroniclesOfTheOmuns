import { apiFetch } from '../auth/apiFetch'
import type { BulkUpdateStats, CharacterStats, SingleUpdateStat } from '../types/characterStats'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export async function bulkUpdateCharacterStats(characterId: number, payload: BulkUpdateStats): Promise<CharacterStats> {
  // PUT /api/character/{characterId}/stats
    const response = await apiFetch(`${BASE_URL}/api/character/${characterId}/stats`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
    })

    if (!response.ok) {
        throw new Error(`Failed to bulk update character stats for character with id ${characterId}: ${response.statusText}`)
    }

    return response.json()
}

export async function getCharacterStats(characterId: number): Promise<CharacterStats> {
  // GET /api/character/{characterId}/stats
    const response = await apiFetch(`${BASE_URL}/api/character/${characterId}/stats`)

    if (!response.ok) {
        throw new Error(`Failed to fetch character stats for character with id ${characterId}: ${response.statusText}`)
    }

    return response.json()
}

export async function updateCharacterStats(characterId: number, payload: SingleUpdateStat): Promise<CharacterStats> {
  // PATCH /api/character/{characterId}/stats
    const response = await apiFetch(`${BASE_URL}/api/character/${characterId}/stats`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
    })

    if (!response.ok) {
        throw new Error(`Failed to update character stats for character with id ${characterId}: ${response.statusText}`)
    }

    return response.json()
}