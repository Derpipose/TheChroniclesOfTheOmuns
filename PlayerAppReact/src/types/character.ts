export type Setting = 'Unregistered' | 'Scifi' | 'Fantasy'

export interface Character {
  id: number
  playerId: number
  name: string
  level: number
  setting: Setting
  currentHealth: number
  isActive: boolean
  notes: string
}

export interface CreateScifiCharacter {
  characterId: number
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
  statPicks?: StatPick[]
  backgroundTalentId?: number | null
}

export interface UpdateScifiCharacter {
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
  statPicks?: StatPick[]
  backgroundTalentId?: number | null
}

export interface ScifiCharacter {
  id: number
  characterId: number
  scifiRaceId?: number
  scifiSubraceId?: number
  scifiClassId?: number
  scifiBackgroundId?: number
  statBonuses: StatModifier[]
  backgroundTalentId: number | null
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