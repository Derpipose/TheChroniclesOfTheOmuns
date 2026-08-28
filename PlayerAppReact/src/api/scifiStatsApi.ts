import { apiFetch } from "../auth/apiFetch"
import type { ScifiStats } from "../types/scifiStats"
const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export async function getAdjustedScifiStatsByCharacterId(characterId: number): Promise<ScifiStats> {
    const response = await apiFetch(`${BASE_URL}/api/scifi/scifiStats/by-character/${characterId}`)

    if (!response.ok) {
        throw new Error(`Failed to fetch adjusted scifi stats for character with id ${characterId}: ${response.statusText}`)
    }

    return response.json()
}