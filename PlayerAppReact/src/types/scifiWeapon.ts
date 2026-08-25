// Enums - string values matching C# API enum names
export const StatType = {
  Strength: 'Strength',
  Constitution: 'Constitution',
  Dexterity: 'Dexterity',
  Wisdom: 'Wisdom',
  Charisma: 'Charisma',
  Intelligence: 'Intelligence',
} as const;
export type StatType = (typeof StatType)[keyof typeof StatType];

export const DamageType = {
  Melee: 'Melee',
  Kinetic: 'Kinetic',
  Energy: 'Energy',
} as const;
export type DamageType = (typeof DamageType)[keyof typeof DamageType];

export const EnergyType = {
  Physical: 'Physical',
  Blaze: 'Blaze',
  Volt: 'Volt',
  Null: 'Null',
  Cryo: 'Cryo',
} as const;
export type EnergyType = (typeof EnergyType)[keyof typeof EnergyType];

export const ScifiAmmoType = {
  None: 'None',
  LesserGeneric: 'LesserGeneric',
  Generic: 'Generic',
  GreaterGeneric: 'GreaterGeneric',
  LesserCommon: 'LesserCommon',
  Common: 'Common',
  GreaterCommon: 'GreaterCommon',
  LesserUncommon: 'LesserUncommon',
  Uncommon: 'Uncommon',
  GreaterUncommon: 'GreaterUncommon',
  LesserRare: 'LesserRare',
  Rare: 'Rare',
  GreaterRare: 'GreaterRare',
  LesserEpic: 'LesserEpic',
  Epic: 'Epic',
  GreaterEpic: 'GreaterEpic',
  LesserLegendary: 'LesserLegendary',
  Legendary: 'Legendary',
  GreaterLegendary: 'GreaterLegendary',
  LesserMythic: 'LesserMythic',
  Mythic: 'Mythic',
  GreaterMythic: 'GreaterMythic',
} as const;
export type ScifiAmmoType = (typeof ScifiAmmoType)[keyof typeof ScifiAmmoType];

export const WeaponRange = {
  None: 'None',
  Melee: 'Melee',
  Short: 'Short',
  Medium: 'Medium',
  Long: 'Long',
} as const;
export type WeaponRange = (typeof WeaponRange)[keyof typeof WeaponRange];

// Blueprint: Shop/Dealer template
export interface ScifiWeapon {
  id: number
  name: string
  damageFormula: string // "1d6"
  modifierStat: StatType
  damageType: DamageType
  energyType: EnergyType
  ammoType: ScifiAmmoType
  ammoCapacity: number
  rateOfFire: string
  reloadTimeInActions: number
  criticalMultiplier: number
  criticalRange: number // lowest critical success roll (e.g., 18, 19, 20)
  criticalFailRange: number | null // highest critical fail roll
  rangeCategory: WeaponRange
}

// Instance: Player-owned weapon (copied from blueprint, then independent)
export interface ScifiWeaponInstance {
  id: number
  name: string
  nickname: string // Player can rename weapon instance
  favorite: boolean // Player can mark weapon instance as favorite
  scifiWeaponId: number // Foreign key to blueprint (for reset-to-factory)
  scifiCharacterId: number // Owner reference
  currentAmmo: number
  damageFormula: string
  modifierStat: StatType
  damageType: DamageType
  energyType: EnergyType
  ammoType: ScifiAmmoType
  ammoCapacity: number
  rateOfFire: string
  reloadTimeInActions: number
  criticalMultiplier: number
  criticalRange: number
  criticalFailRange: number | null
  rangeCategory: WeaponRange
}

// DTOs for weapon blueprint creation/update
export interface CreateScifiWeaponDto {
  name: string
  damageFormula: string
  ammoType: ScifiAmmoType
  damageType: DamageType
  energyType: EnergyType
  rateOfFire: string
  ammoCapacity: number
  criticalRange: number
  criticalMultiplier: number
  reloadTimeInActions?: number
  rangeCategory?: WeaponRange
  criticalFailRange?: number
  modifierStat?: StatType
}

export interface UpdateScifiWeaponDto {
  name?: string
  damageFormula?: string
  ammoType?: ScifiAmmoType
  damageType?: DamageType
  energyType?: EnergyType
  rateOfFire?: string
  ammoCapacity?: number
  criticalRange?: number
  criticalMultiplier?: number
  reloadTimeInActions?: number
  rangeCategory?: WeaponRange
  criticalFailRange?: number | null
  modifierStat?: StatType
}

// DTOs for weapon instance creation
export interface CreateScifiWeaponInstanceDto {
  scifiWeaponId: number // Blueprint ID to copy from
  scifiCharacterId: number // Character who owns this instance
  scifiWeaponBlueprintId: number // Blueprint ID to copy from

}

// DTOs for weapon instance updates
export interface StringUpdateDto {
  newValue: string
}

export interface IntUpdateDto {
  newValue: number
}

export interface UpdateWeaponDamageTypeDto {
  damageType: DamageType
}

export interface UpdateWeaponEnergyTypeDto {
  energyType: EnergyType
}

export interface UpdateWeaponModifierStatDto {
  stat: StatType
}

export interface UpdateWeaponAmmoTypeDto {
  ammoType: ScifiAmmoType
}

export interface UpdateWeaponRangeCategoryDto {
  range: WeaponRange
}
