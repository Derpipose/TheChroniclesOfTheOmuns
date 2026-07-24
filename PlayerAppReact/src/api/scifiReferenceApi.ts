import { apiFetch } from '../auth/apiFetch'
import type { ScifiRace, ScifiSubrace, ScifiClass, ScifiBackground } from '../types/scifiReference'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export async function getScifiRaces(): Promise<ScifiRace[]> {
  // GET /api/scifi/ScifiRace
    const response = await apiFetch(`${BASE_URL}/api/scifi/ScifiRace`)

    if (!response.ok) {
        throw new Error(`Failed to fetch scifi races: ${response.statusText}`)
    }

    return response.json()
}

export async function getScifiSubracesByRace(raceId: number): Promise<ScifiSubrace[]> {
  // GET /api/scifi/ScifiRace/subrace/race/{raceId}
    const response = await apiFetch(`${BASE_URL}/api/scifi/ScifiRace/subrace/race/${raceId}`)

    if (response.status === 404) {
        return [] // Return an empty array if no subraces are found for the given raceId
    }
    
    if (!response.ok) {
        throw new Error(`Failed to fetch scifi subraces for race with id ${raceId}: ${response.statusText}`)
    }

    return response.json()
}

export async function getScifiClasses(): Promise<ScifiClass[]> {
  // GET /api/scifi/ScifiClass
    const response = await apiFetch(`${BASE_URL}/api/scifi/ScifiClass`)

    if (!response.ok) {
        throw new Error(`Failed to fetch scifi classes: ${response.statusText}`)
    }

    return response.json()
}

export async function getScifiBackgrounds(): Promise<ScifiBackground[]> {
  // GET /api/scifi/ScifiBackground
    const response = await apiFetch(`${BASE_URL}/api/scifi/ScifiBackground`)

    if (!response.ok) {
        throw new Error(`Failed to fetch scifi backgrounds: ${response.statusText}`)
    }

    return response.json()
}