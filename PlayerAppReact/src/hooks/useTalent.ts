import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getTalentPicksByCharacterId, selectTalentForCharacter, previewScifiTalentPicks } from '../api/scifiTalentApi';
import type { ScifiTalentPick, ScifiTalentPreview } from '../types/scifiTalent';

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

export function useScifiTalentPicksPreview(scifiClassId?: number, scifiBackgroundId?: number) {
  return useQuery<ScifiTalentPreview[]>({
    queryKey: ['scifiTalentPicksPreview', scifiClassId, scifiBackgroundId],
    queryFn: () => previewScifiTalentPicks(Number(scifiClassId), Number(scifiBackgroundId)),
    enabled: !!scifiClassId && !!scifiBackgroundId,
    staleTime: Infinity,
  });
}