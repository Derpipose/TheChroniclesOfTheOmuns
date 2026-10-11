import { useState } from 'react'
import Card from '../Card'
import { useDeleteScifiWeapon, useScifiWeapons } from '../../hooks/useScifiWeapon'

/** Search for a weapon blueprint and remove it from the database. */
export default function DeleteWeaponCard() {
  const deleteScifiWeapon = useDeleteScifiWeapon()
  const { data: scifiWeapons = [] } = useScifiWeapons()

  const [weaponSearch, setWeaponSearch] = useState('')
  const [selectedWeaponId, setSelectedWeaponId] = useState<number | ''>('')

  const filteredWeapons = scifiWeapons.filter((weapon) =>
    weapon.name.toLowerCase().includes(weaponSearch.trim().toLowerCase())
  )

  const handleDeleteWeapon = () => {
    if (!selectedWeaponId) return

    const selectedWeapon = scifiWeapons.find(
      (weapon) => weapon.id === selectedWeaponId
    )
    if (!selectedWeapon) return

    if (!window.confirm(`Delete the ${selectedWeapon.name} blueprint?`)) return

    deleteScifiWeapon.mutate(selectedWeapon.id, {
      onSuccess: () => setSelectedWeaponId(''),
    })
  }

  return (
    <Card
      className="flex flex-col gap-4"
      title="Delete Weapon Blueprint"
      description="Search for a blueprint, select it, and remove it from the database."
    >
      <input
        type="text"
        value={weaponSearch}
        onChange={(event) => setWeaponSearch(event.target.value)}
        placeholder="Search weapon blueprints"
        className="field-input"
      />

      <select
        value={selectedWeaponId}
        onChange={(event) =>
          setSelectedWeaponId(event.target.value ? Number(event.target.value) : '')
        }
        className="field-input"
      >
        <option value="">Select a weapon blueprint</option>
        {filteredWeapons.map((weapon) => (
          <option key={weapon.id} value={weapon.id}>{weapon.name}</option>
        ))}
      </select>

      <button
        type="button"
        onClick={handleDeleteWeapon}
        disabled={!selectedWeaponId || deleteScifiWeapon.isPending}
        className="btn btn-danger border"
      >
        {deleteScifiWeapon.isPending ? 'Deleting...' : 'Delete Blueprint'}
      </button>

      {deleteScifiWeapon.isError && (
        <p className="text-xs fg-danger">
          Failed to delete weapon blueprint. Please try again.
        </p>
      )}
    </Card>
  )
}
