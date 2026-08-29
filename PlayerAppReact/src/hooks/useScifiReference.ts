import { useQuery } from '@tanstack/react-query'
import type { ScifiRace } from '../types/scifiReference'
import { getScifiBackgrounds, getScifiClasses, getScifiRaces, getScifiSubracesByRace } from '../api/scifiReferenceApi'

export function useScifiRaces() {
    // ...
    return useQuery<ScifiRace[]>({
        queryKey: ['scifiRaces'],
        queryFn: getScifiRaces,
    })
}

export function useScifiSubracesByRace(raceId: number | undefined) {
    // needs queryKey that includes raceId (so each race's subraces cache separately)
    // needs enabled: raceId !== undefined
    // needs queryFn that only calls getScifiSubracesByRace if raceId is defined
    return useQuery({
        queryKey: ['scifiSubraces', raceId],
        queryFn: () => {
            if (raceId === undefined) {
                throw new Error('raceId is undefined')
            }
            return getScifiSubracesByRace(raceId)
        },
        enabled: raceId !== undefined,
    })
}

export function useScifiClasses() {
    // ...
    return useQuery({
        queryKey: ['scifiClasses'],
        queryFn: getScifiClasses,
    })
}

export function useScifiBackgrounds() {
    // ...
    return useQuery({
        queryKey: ['scifiBackgrounds'],
        queryFn: getScifiBackgrounds,
    })
}