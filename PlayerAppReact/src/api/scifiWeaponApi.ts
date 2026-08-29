import { apiFetch } from '../auth/apiFetch'
import type { ScifiWeapon, CreateScifiWeaponDto, UpdateScifiWeaponDto } from '../types/scifiWeapon'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export async function getAllScifiWeapons(): Promise<ScifiWeapon[]> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weapon`)
  if (!res.ok) throw new Error('Failed to fetch weapons')
  return res.json()
}

export async function getScifiWeapon(id: number): Promise<ScifiWeapon> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weapon/${id}`)
  if (!res.ok) throw new Error('Failed to fetch weapon')
  return res.json()
}

export async function createScifiWeapon(payload: CreateScifiWeaponDto): Promise<ScifiWeapon> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weapon`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error('Failed to create weapon')
  return res.json()
}

export async function updateScifiWeapon(id: number, payload: UpdateScifiWeaponDto): Promise<ScifiWeapon> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weapon/${id}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error('Failed to update weapon')
  return res.json()
}

export async function deleteScifiWeapon(id: number): Promise<void> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/weapon/${id}`, {
    method: 'DELETE',
  })
  if (!res.ok) throw new Error('Failed to delete weapon')
}
