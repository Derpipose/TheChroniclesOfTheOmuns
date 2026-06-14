export interface Character {
  id: number
  playerId: number
  name: string
  level: number
  setting: string
  maxHealth: number
}

export interface ScifiCharacter {
  id: number
  characterId: number
  character: Character
}

// Union for when you need to handle multiple subtypes
export type AnyCharacter =
  | { type: 'scifi'; data: ScifiCharacter }
