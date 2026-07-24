import { useState } from 'react'
import type { SeedResponse, SeedResult, SeedStatus } from '../types/devSeed'
import { getAuthHeader } from '../auth/authHeader'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export function useDevSeeding() {
    const [status, setStatus] = useState<SeedResult>({
        talents: 'idle',
        classes: 'idle',
        backgrounds: 'idle',
        races: 'idle',
        subraces: 'idle',
    })

    // Holds the last response/error message per table so skipped items are visible, not just swallowed.
    const [messages, setMessages] = useState<Partial<Record<keyof SeedResult, string>>>({})

    const setTableStatus = (table: keyof SeedResult, value: SeedStatus) =>
        setStatus(prev => ({ ...prev, [table]: value }))

    const setTableMessage = (table: keyof SeedResult, message: string) =>
        setMessages(prev => ({ ...prev, [table]: message }))

    // Shared runner for the three ScifiSeedingController endpoints — same auth,
    // same bodyless POST, same success/error shape.
    const runSeed = async (table: keyof SeedResult, endpoint: string) => {
        setTableStatus(table, 'loading')
        setTableMessage(table, '')
        try {
        const response = await fetch(`${BASE_URL}/api/scifi/scifiseeding/${endpoint}`, {
            method: 'POST',
            headers: getAuthHeader(),
        })

        if (!response.ok) {
            const errorBody = await response.text()
            setTableMessage(table, errorBody || `Request failed (${response.status})`)
            setTableStatus(table, 'error')
            return
        }

        const data: SeedResponse = await response.json()
        const skipped = data.skippedClasses ?? data.skippedTalents ?? []
        const summary = `Created ${data.created}, updated ${data.updated}, total ${data.total}` +
            (skipped.length > 0 ? ` — skipped: ${skipped.join(', ')}` : '')
        setTableMessage(table, summary)
        setTableStatus(table, 'success')
        } catch (err) {
        setTableMessage(table, err instanceof Error ? err.message : 'Unknown error')
        setTableStatus(table, 'error')
        }
    }

    const seedTalents = () => runSeed('talents', 'talents')
    const seedClasses = () => runSeed('classes', 'classes')
    const seedBackgrounds = () => runSeed('backgrounds', 'backgrounds')

    const seedRaces = async () => {
        setTableStatus('races', 'loading')
        try {
        // TODO: Wire in Josh's race endpoints here
        await fetch(`${BASE_URL}/api/scifi/ScifiRace/seed`, {
            method: 'POST',
            headers: getAuthHeader(),
        })
        setTableStatus('races', 'success')
        } catch {
        setTableStatus('races', 'error')
        }
    }

    const seedSubraces = async () => {
        setTableStatus('subraces', 'loading')
        try {
        // TODO: Wire in Josh's subrace endpoints here
        await fetch(`${BASE_URL}/api/scifi/ScifiRace/seed`, {
            method: 'POST',
            headers: getAuthHeader(),
        })
        setTableStatus('subraces', 'success')
        } catch {
        setTableStatus('subraces', 'error')
        }
    }

    return {
        status,
        messages,
        seedTalents,
        seedClasses,
        seedBackgrounds,
        seedRaces,
        seedSubraces,
    }
}