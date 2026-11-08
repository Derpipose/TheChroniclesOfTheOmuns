import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getTalentPicksByCharacterId, selectTalentForCharacter } from '../api/scifiTalentApi';
import type { ScifiTalentPick } from '../types/scifiTalent';

export function useTalentPicksByCharacterId(characterId: number, scifiClassId: string | null, scifiBackgroundId: string | null) {
  return useQuery<ScifiTalentPick[]>({
    queryKey: ['talentPicks', characterId, scifiClassId, scifiBackgroundId],
    queryFn: () => getTalentPicksByCharacterId(characterId),
    enabled: !!characterId && !!scifiClassId && !!scifiBackgroundId,
  });
}

export function useSelectTalentPick(characterId: number){
  const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (talentId: number) => selectTalentForCharacter(characterId, talentId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['talentPicks', characterId] });
        },
    });
}