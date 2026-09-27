export type Setting = 'Unregistered' | 'Scifi' | 'Fantasy'

export interface Character {
  id: number
  playerId: number
  name: string
  level: number
  setting: Setting
  maxHealth: number
  active: boolean
  notes: string
}

export interface CreateScifiCharacter {
  characterId: number
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
  statPicks?: StatPick[]
}

export interface UpdateScifiCharacter {
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
  statPicks?: StatPick[]
}

export interface ScifiCharacter {
  id: number
  characterId: number
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
  statBonuses: StatModifier[]
}

// Union for when you need to handle multiple subtypes
export type AnyCharacter =
  { type: 'scifi'; data: ScifiCharacter }

export interface StatPick {
  stat: string
  amount: number
}

export interface StatModifier {
  stat: string
  amount: number
  source: string
}