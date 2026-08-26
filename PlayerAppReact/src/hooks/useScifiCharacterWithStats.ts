

import { useQuery } from '@tanstack/react-query'
import { getScifiCharacterWithAdjustedStats } from '../api/scifiCharacterApi'
import type { CharacterStats } from '../types/characterStats'

export function useScifiCharacterWithStats(scifiCharacterId?: number) {
	return useQuery<CharacterStats | null>({
		queryKey: ['scifiCharacterWithStats', scifiCharacterId],
		queryFn: () => getScifiCharacterWithAdjustedStats(scifiCharacterId!),
		enabled: typeof scifiCharacterId === 'number' && scifiCharacterId > 0,
	})
}

