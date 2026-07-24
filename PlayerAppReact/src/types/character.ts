export type Setting = 'Unregistered' | 'Scifi' | 'Fantasy'

export interface Character {
  id: number
  playerId: number
  name: string
  level: number
  setting: Setting
  maxHealth: number
  active: boolean
}

export interface CreateScifiCharacter {
  characterId: number
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
}

export interface UpdateScifiCharacter {
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
}

export interface ScifiCharacter {
  id: number
  characterId: number
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
}

// Union for when you need to handle multiple subtypes
export type AnyCharacter =
  | { type: 'scifi'; data: ScifiCharacter }
