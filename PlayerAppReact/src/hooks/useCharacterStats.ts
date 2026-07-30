import { useQuery } from '@tanstack/react-query'
import { getCharacterStats } from '../api/characterStatsApi'
import type { CharacterStats } from '../types/characterStats'

export function useCharacterStats(characterId: number) {
    return useQuery<CharacterStats>({
        queryKey: ['characterStats', characterId],
        queryFn: () => getCharacterStats(characterId),
        enabled: !!characterId,
    })
}