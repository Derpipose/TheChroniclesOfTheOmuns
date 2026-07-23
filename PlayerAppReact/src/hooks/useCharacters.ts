import { useQuery } from '@tanstack/react-query'
import { getCharacters } from '../api/characterApi'
import type { Character } from '../types/character'

export function useCharacters() {
  return useQuery<Character[]>({
    queryKey: ['characters'],
    queryFn: getCharacters,
  })
}