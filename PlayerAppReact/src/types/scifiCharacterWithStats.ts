export interface AdjustedStats {
  strength: number
  dexterity: number
  constitution: number
  intelligence: number
  wisdom: number
  charisma: number
  armorClass: number
  damageResist:number
  maxHealth: number
  currentHealth: number
}

export interface StatModifierDto {
  stat: number
  amount: number
  source: number
}

export interface ScifiCharacterWithStats {
  id: number
  characterId: number
  scifiRaceId?: number | null
  scifiSubraceId?: number | null
  scifiClassId?: number | null
  scifiBackgroundId?: number | null
  adjustedStats: AdjustedStats
  modifiers: StatModifierDto[]
}
