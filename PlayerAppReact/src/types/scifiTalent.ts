export interface ScifiTalent {
  id: number
  name: string
  description: string
  ability: string
  perk: string
  perkAmount: string
}

export interface ScifiTalentPick extends ScifiTalent {
  isPickable: boolean
  isSelected: boolean
}

export interface ScifiTalentPreview extends ScifiTalent {
  isPickable: boolean
}

