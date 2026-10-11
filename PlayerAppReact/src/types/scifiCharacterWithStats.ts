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