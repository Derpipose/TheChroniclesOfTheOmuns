import { useQuery } from '@tanstack/react-query'
import { getCharacterById, getCharacters } from '../api/characterApi'
import type { Character } from '../types/character'
import { getScifiCharacterByCharacterId } from '../api/scifiCharacterApi'

export function useCharacters() {
  return useQuery<Character[]>({
    queryKey: ['characters'],
    queryFn: getCharacters,
  })
}

export function useScifiCharacterByCharacterId(characterId: number) {
  return useQuery({
    queryKey: ['scifiCharacter', characterId],
    queryFn: () => getScifiCharacterByCharacterId(characterId),
    enabled: !!characterId,
  })
}

export function useCharacter(id: number) {
  return useQuery<Character>({
    queryKey: ['character', id],
    queryFn: () => getCharacterById(id),
    enabled: !!id,
  })
}