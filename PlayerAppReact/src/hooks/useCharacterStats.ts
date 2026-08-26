import { useQuery } from '@tanstack/react-query'
import { getCharacterStats } from '../api/characterStatsApi'
import type { CharacterStats } from '../types/characterStats'
import { getScifiCharacterWithAdjustedStats } from '../api/scifiCharacterApi'
import type { CharacterStats as CharacterStatsType } from '../types/characterStats'

type ScifiRef = number | { id?: number; characterId?: number } | undefined | null

function resolveScifiId(ref: ScifiRef): number | undefined {
  if (ref == null) return undefined
  if (typeof ref === 'number') return ref
  return ref.id ?? ref.characterId
}

export function useCharacterStats(characterId: number) {
    return useQuery<CharacterStats>({
        queryKey: ['characterStats', characterId],
        queryFn: () => getCharacterStats(characterId),
        enabled: !!characterId,
    })
}



export function useScifiCharacterWithStats(scifiCharacterRef?: ScifiRef) {
  const resolvedId = resolveScifiId(scifiCharacterRef)

  return useQuery<CharacterStatsType | null>({
    queryKey: ['scifiCharacterWithStats', resolvedId],
    queryFn: () => getScifiCharacterWithAdjustedStats(resolvedId!),
    enabled: typeof resolvedId === 'number' && resolvedId > 0,
  })
}
