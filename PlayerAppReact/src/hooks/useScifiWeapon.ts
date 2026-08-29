
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  getAllScifiWeapons,
  getScifiWeapon,
  createScifiWeapon,
  updateScifiWeapon,
  deleteScifiWeapon,
} from '../api/scifiWeaponApi'
import type {
  ScifiWeapon,
  CreateScifiWeaponDto,
  UpdateScifiWeaponDto,
} from '../types/scifiWeapon'

export function useScifiWeapons() {
  return useQuery<ScifiWeapon[]>({
    queryKey: ['scifiWeapons'],
    queryFn: getAllScifiWeapons,
  })
}

export function useScifiWeapon(id: number) {
  return useQuery<ScifiWeapon>({
    queryKey: ['scifiWeapon', id],
    queryFn: () => getScifiWeapon(id),
    enabled: !!id,
  })
}

export function useCreateScifiWeapon() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateScifiWeaponDto) =>
      createScifiWeapon(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['scifiWeapons'] })
    },
  })
}

export function useUpdateScifiWeapon() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      id,
      payload,
    }: {
      id: number
      payload: UpdateScifiWeaponDto
    }) => updateScifiWeapon(id, payload),
    onSuccess: (weapon) => {
      queryClient.invalidateQueries({ queryKey: ['scifiWeapons'] })
      queryClient.invalidateQueries({
        queryKey: ['scifiWeapon', weapon.id],
      })
    },
  })
}

export function useDeleteScifiWeapon() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteScifiWeapon(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['scifiWeapons'] })
      queryClient.removeQueries({ queryKey: ['scifiWeapon', id] })
    },
  })
}

