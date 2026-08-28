import { useQuery } from '@tanstack/react-query'
import { getAdjustedScifiStatsByCharacterId } from '../api/scifiStatsApi'

export function useScifiStatsByCharacterId(characterId: number | undefined) {
    return useQuery({
        queryKey: ['scifiStats', characterId],
        queryFn: () => {
            if (characterId === undefined) {
                throw new Error('characterId is undefined')
            }
            return getAdjustedScifiStatsByCharacterId(characterId)
        },
        enabled: characterId !== undefined,
    })
}