export interface BulkUpdateStats {
    strength: number
    constitution: number
    dexterity: number
    wisdom: number
    charisma: number
    intelligence: number
}

export type StatName = 'Strength' | 'Constitution' | 'Dexterity' | 'Wisdom' | 'Charisma' | 'Intelligence'

export interface SingleUpdateStat {
    stat: StatName
    value: number
}

export interface CharacterStats {
    id: number
    characterId: number
    strength: number
    constitution: number
    dexterity: number
    wisdom: number
    charisma: number
    intelligence: number
}