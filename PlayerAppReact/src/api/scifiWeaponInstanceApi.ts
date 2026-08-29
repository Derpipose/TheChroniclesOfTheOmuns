import { apiFetch } from '../auth/apiFetch'
import type {
  ScifiWeaponInstance,
  CreateScifiWeaponInstanceDto,
  StringUpdateDto,
  IntUpdateDto,
  UpdateWeaponDamageTypeDto,
  UpdateWeaponEnergyTypeDto,
  UpdateWeaponModifierStatDto,
  UpdateWeaponAmmoTypeDto,
  UpdateWeaponRangeCategoryDto,
} from '../types/scifiWeapon'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

// CRUD Operations
export async function createScifiWeaponInstance(
  payload: CreateScifiWeaponInstanceDto
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error('Failed to create weapon instance')
  return res.json()
}

export async function getScifiWeaponInstance(id: number): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}`)
  if (!res.ok) throw new Error('Failed to fetch weapon instance')
  return res.json()
}

export async function getUsersScifiWeaponInstances(
  characterId: number
): Promise<ScifiWeaponInstance[]> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/character/${characterId}`)
  if (!res.ok) throw new Error('Failed to fetch weapon instances')
  return res.json()
}

export async function deleteScifiWeaponInstance(id: number): Promise<void> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}`, {
    method: 'DELETE',
  })
  if (!res.ok) throw new Error('Failed to delete weapon instance')
}

// Player Customization Methods
export async function renameWeapon(
  id: number,
  newName: string
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/rename`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: newName } as StringUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to rename weapon')
  return res.json()
}

export async function toggleFavorite(id: number): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/favorite`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
  })
  if (!res.ok) throw new Error('Failed to toggle favorite')
  return res.json()
}

export async function changeOwner(
  id: number,
  newCharacterId: number
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/owner`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: newCharacterId } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to change owner')
  return res.json()
}

// Ammo Management
export async function useAmmo(id: number, amount: number): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/use-ammo`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: amount } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to use ammo')
  return res.json()
}

export async function reload(id: number): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/reload`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
  })
  if (!res.ok) throw new Error('Failed to reload weapon')
  return res.json()
}

export async function setAmmo(id: number, amount: number): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/ammo`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: amount } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set ammo')
  return res.json()
}

// Damage & Combat Stats
export async function setDamageFormula(
  id: number,
  formula: string
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/damage-formula`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: formula } as StringUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set damage formula')
  return res.json()
}

export async function setDamageType(
  id: number,
  damageType: import('../types/scifiWeapon').DamageType
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/damage-type`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ damageType } as UpdateWeaponDamageTypeDto),
  })
  if (!res.ok) throw new Error('Failed to set damage type')
  return res.json()
}

export async function setEnergyType(
  id: number,
  energyType: import('../types/scifiWeapon').EnergyType
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/energy-type`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ energyType } as UpdateWeaponEnergyTypeDto),
  })
  if (!res.ok) throw new Error('Failed to set energy type')
  return res.json()
}

export async function setModifierStat(
  id: number,
  stat: import('../types/scifiWeapon').StatType
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/modifier-stat`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ stat } as UpdateWeaponModifierStatDto),
  })
  if (!res.ok) throw new Error('Failed to set modifier stat')
  return res.json()
}

export async function setRateOfFire(
  id: number,
  rateOfFire: string
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/rate-of-fire`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: rateOfFire } as StringUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set rate of fire')
  return res.json()
}

// Ammo Handling
export async function setAmmoType(
  id: number,
  ammoType: import('../types/scifiWeapon').ScifiAmmoType
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/ammo-type`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ ammoType } as UpdateWeaponAmmoTypeDto),
  })
  if (!res.ok) throw new Error('Failed to set ammo type')
  return res.json()
}

export async function setAmmoCapacity(
  id: number,
  capacity: number
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/ammo-capacity`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: capacity } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set ammo capacity')
  return res.json()
}

export async function setReloadTime(
  id: number,
  reloadTime: number
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/reload-time`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: reloadTime } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set reload time')
  return res.json()
}

// Critical Hit Stats
export async function setCriticalRange(
  id: number,
  range: number
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/critical-range`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: range } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set critical range')
  return res.json()
}

export async function setCriticalMultiplier(
  id: number,
  multiplier: number
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/critical-multiplier`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: multiplier } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set critical multiplier')
  return res.json()
}

export async function setCriticalFailRange(
  id: number,
  range: number
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/critical-fail-range`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ newValue: range } as IntUpdateDto),
  })
  if (!res.ok) throw new Error('Failed to set critical fail range')
  return res.json()
}

export async function setRangeCategory(
  id: number,
  range: import('../types/scifiWeapon').WeaponRange
): Promise<ScifiWeaponInstance> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weaponinstance/${id}/range-category`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ range } as UpdateWeaponRangeCategoryDto),
  })
  if (!res.ok) throw new Error('Failed to set range category')
  return res.json()
}
