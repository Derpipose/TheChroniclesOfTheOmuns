import { useQuery } from '@tanstack/react-query'
import { getCharacterStats } from '../api/characterStatsApi'
import type { CharacterStats } from '../types/characterStats'
import { getScifiCharacterWithAdjustedStats } from '../api/scifiCharacterApi'
import type { CharacterStats as CharacterStatsType } from '../types/characterStats'

export function useCharacterStats(characterId: number) {
    return useQuery<CharacterStats>({
        queryKey: ['characterStats', characterId],
        queryFn: () => getCharacterStats(characterId),
        enabled: !!characterId,
    })
}



export function useScifiCharacterWithStats(scifiCharacterId?: number) {
  return useQuery<CharacterStatsType | null>({
    queryKey: ['scifiCharacterWithStats', scifiCharacterId],
    queryFn: () => getScifiCharacterWithAdjustedStats(scifiCharacterId!),
    enabled: typeof scifiCharacterId === 'number' && scifiCharacterId > 0,
  })
}
