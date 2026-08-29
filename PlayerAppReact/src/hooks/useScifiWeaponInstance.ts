import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createScifiWeaponInstance,
  getScifiWeaponInstance,
  getUsersScifiWeaponInstances,
  deleteScifiWeaponInstance,
  renameWeapon,
  toggleFavorite,
  changeOwner,
  useAmmo as fireAmmo,
  reload,
  setAmmo,
  setDamageFormula,
  setDamageType,
  setEnergyType,
  setModifierStat,
  setRateOfFire,
  setAmmoType,
  setAmmoCapacity,
  setReloadTime,
  setCriticalRange,
  setCriticalMultiplier,
  setCriticalFailRange,
  setRangeCategory,
} from '../api/scifiWeaponInstanceApi'
import type {
  ScifiWeaponInstance,
  CreateScifiWeaponInstanceDto,
  DamageType,
  EnergyType,
  StatType,
  ScifiAmmoType,
  WeaponRange,
} from '../types/scifiWeapon'

export function useScifiWeaponInstance(id: number) {
  return useQuery<ScifiWeaponInstance>({
    queryKey: ['scifiWeaponInstance', id],
    queryFn: () => getScifiWeaponInstance(id),
    enabled: !!id,
  })
}

export function useScifiWeaponInstances(characterId: number) {
  return useQuery<ScifiWeaponInstance[]>({
    queryKey: ['scifiWeaponInstances', characterId],
    queryFn: () => getUsersScifiWeaponInstances(characterId),
    enabled: !!characterId,
  })
}

export function useCreateScifiWeaponInstance() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateScifiWeaponInstanceDto) =>
      createScifiWeaponInstance(payload),
    onSuccess: (weapon) => {
      queryClient.invalidateQueries({
        queryKey: ['scifiWeaponInstances'],
      })
      queryClient.setQueryData(
        ['scifiWeaponInstance', weapon.id],
        weapon
      )
    },
  })
}

export function useDeleteScifiWeaponInstance() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteScifiWeaponInstance(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({
        queryKey: ['scifiWeaponInstances'],
      })
      queryClient.removeQueries({
        queryKey: ['scifiWeaponInstance', id],
      })
    },
  })
}

function useWeaponInstanceMutation<TVariables>(
  mutationFn: (variables: TVariables) => Promise<ScifiWeaponInstance>,
  options?: {
    onSuccess?: (weapon: ScifiWeaponInstance) => void
  }
) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn,
    onSuccess: (weapon) => {
      queryClient.setQueryData(
        ['scifiWeaponInstance', weapon.id],
        weapon
      )
      // Invalidate all scifiWeaponInstances queries (with any character ID)
      queryClient.invalidateQueries({
        queryKey: ['scifiWeaponInstances'],
      })
      // Call any additional onSuccess callback
      options?.onSuccess?.(weapon)
    },
  })
}

export function useRenameWeapon() {
  return useWeaponInstanceMutation(
    ({ id, newName }: { id: number; newName: string }) =>
      renameWeapon(id, newName)
  )
}

export function useToggleFavorite() {
  return useWeaponInstanceMutation(
    (id: number) => toggleFavorite(id)
  )
}

export function useChangeWeaponOwner() {
  return useWeaponInstanceMutation(
    ({
      id,
      newCharacterId,
    }: {
      id: number
      newCharacterId: number
    }) => changeOwner(id, newCharacterId)
  )
}

export function useUseAmmo() {
  return useWeaponInstanceMutation(
    ({ id, amount }: { id: number; amount: number }) =>
      fireAmmo(id, amount)
  )
}

export function useReloadWeapon() {
  return useWeaponInstanceMutation(
    (id: number) => reload(id)
  )
}

export function useSetAmmo() {
  return useWeaponInstanceMutation(
    ({ id, amount }: { id: number; amount: number }) =>
      setAmmo(id, amount)
  )
}

export function useSetDamageFormula() {
  return useWeaponInstanceMutation(
    ({ id, formula }: { id: number; formula: string }) =>
      setDamageFormula(id, formula)
  )
}

export function useSetDamageType() {
  return useWeaponInstanceMutation(
    ({ id, damageType }: { id: number; damageType: DamageType }) =>
      setDamageType(id, damageType)
  )
}

export function useSetEnergyType() {
  return useWeaponInstanceMutation(
    ({ id, energyType }: { id: number; energyType: EnergyType }) =>
      setEnergyType(id, energyType)
  )
}

export function useSetModifierStat() {
  return useWeaponInstanceMutation(
    ({ id, stat }: { id: number; stat: StatType }) =>
      setModifierStat(id, stat)
  )
}

export function useSetRateOfFire() {
  return useWeaponInstanceMutation(
    ({ id, rateOfFire }: { id: number; rateOfFire: string }) =>
      setRateOfFire(id, rateOfFire)
  )
}

export function useSetAmmoType() {
  return useWeaponInstanceMutation(
    ({ id, ammoType }: { id: number; ammoType: ScifiAmmoType }) =>
      setAmmoType(id, ammoType)
  )
}

export function useSetAmmoCapacity() {
  return useWeaponInstanceMutation(
    ({ id, capacity }: { id: number; capacity: number }) =>
      setAmmoCapacity(id, capacity)
  )
}

export function useSetReloadTime() {
  return useWeaponInstanceMutation(
    ({ id, reloadTime }: { id: number; reloadTime: number }) =>
      setReloadTime(id, reloadTime)
  )
}

export function useSetCriticalRange() {
  return useWeaponInstanceMutation(
    ({ id, range }: { id: number; range: number }) =>
      setCriticalRange(id, range)
  )
}

export function useSetCriticalMultiplier() {
  return useWeaponInstanceMutation(
    ({ id, multiplier }: { id: number; multiplier: number }) =>
      setCriticalMultiplier(id, multiplier)
  )
}

export function useSetCriticalFailRange() {
  return useWeaponInstanceMutation(
    ({ id, range }: { id: number; range: number }) =>
      setCriticalFailRange(id, range)
  )
}

export function useSetRangeCategory() {
  return useWeaponInstanceMutation(
    ({ id, range }: { id: number; range: WeaponRange }) =>
      setRangeCategory(id, range)
  )
}
