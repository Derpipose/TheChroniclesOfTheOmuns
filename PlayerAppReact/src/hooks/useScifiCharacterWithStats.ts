import { useQuery } from '@tanstack/react-query'
import { getScifiCharacterWithAdjustedStats } from '../api/scifiCharacterApi'
import type { CharacterStats } from '../types/characterStats'


export function useScifiCharacterWithStats(characterID: number | undefined) {
	const resolvedId = characterID

	return useQuery<CharacterStats | null>({
		queryKey: ['scifiCharacterWithStats', resolvedId],
		queryFn: () => {
			if (resolvedId == null) {
				throw new Error('A character ID is required')
			}

			return getScifiCharacterWithAdjustedStats(resolvedId)
		},
		enabled: resolvedId != null && resolvedId > 0,
	})
}

