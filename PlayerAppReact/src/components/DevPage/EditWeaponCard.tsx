import { useState } from 'react'
import Card from '../Card'
import { useScifiWeapons, useUpdateScifiWeapon } from '../../hooks/useScifiWeapon'
import {
  StatType,
  DamageType,
  EnergyType,
  ScifiAmmoType,
  WeaponRange,
  type ScifiWeapon,
} from '../../types/scifiWeapon'
import EnumSelect from './EnumSelect'
import Field from './Field'

type WeaponDraft = Omit<ScifiWeapon, 'id'>

/** Search for a weapon blueprint and edit its database values. */
export default function EditWeaponCard() {
  const updateScifiWeapon = useUpdateScifiWeapon()
  const { data: scifiWeapons = [] } = useScifiWeapons()

  const [weaponSearch, setWeaponSearch] = useState('')
  const [selectedWeaponId, setSelectedWeaponId] = useState<number | ''>('')
  const [draft, setDraft] = useState<WeaponDraft | null>(null)

  const filteredWeapons = scifiWeapons.filter((weapon) =>
    weapon.name.toLowerCase().includes(weaponSearch.trim().toLowerCase())
  )

  const canUpdateWeapon =
    draft !== null &&
    draft.name.trim() !== '' &&
    draft.damageFormula.trim() !== '' &&
    draft.rateOfFire.trim() !== ''

  const handleSelectWeapon = (value: string) => {
    const weaponId = value ? Number(value) : ''
    setSelectedWeaponId(weaponId)

    const selectedWeapon = scifiWeapons.find((weapon) => weapon.id === weaponId)
    if (!selectedWeapon) {
      setDraft(null)
      return
    }

    const { id: _id, ...weaponDraft } = selectedWeapon
    setDraft(weaponDraft)
  }

  const handleUpdateWeapon = () => {
    if (!selectedWeaponId || !draft || !canUpdateWeapon) return

    updateScifiWeapon.mutate(
      {
        id: selectedWeaponId,
        payload: {
          ...draft,
          name: draft.name.trim(),
          damageFormula: draft.damageFormula.trim(),
          rateOfFire: draft.rateOfFire.trim(),
        },
      }
    )
  }

  return (
    <Card
      className="flex flex-col gap-4"
      title="Edit Weapon Blueprint"
      description="Search for a blueprint, select it, and update its database values."
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
        onChange={(event) => handleSelectWeapon(event.target.value)}
        className="field-input"
      >
        <option value="">Select a weapon blueprint</option>
        {filteredWeapons.map((weapon) => (
          <option key={weapon.id} value={weapon.id}>{weapon.name}</option>
        ))}
      </select>

      {draft && (
        <>
          <Field label="Name">
            <input
              type="text"
              value={draft.name}
              onChange={(event) => setDraft({ ...draft, name: event.target.value })}
              className="field-input"
            />
          </Field>

          <div className="grid grid-cols-2 gap-3">
            <Field label="Damage Formula">
              <input
                type="text"
                value={draft.damageFormula}
                onChange={(event) => setDraft({ ...draft, damageFormula: event.target.value })}
                className="field-input"
              />
            </Field>
            <Field label="Rate of Fire">
              <input
                type="text"
                value={draft.rateOfFire}
                onChange={(event) => setDraft({ ...draft, rateOfFire: event.target.value })}
                className="field-input"
              />
            </Field>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <EnumSelect label="Damage Type" value={draft.damageType} options={DamageType} onChange={(damageType) => setDraft({ ...draft, damageType })} />
            <EnumSelect label="Energy Type" value={draft.energyType} options={EnergyType} onChange={(energyType) => setDraft({ ...draft, energyType })} />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <EnumSelect label="Ammo Type" value={draft.ammoType} options={ScifiAmmoType} onChange={(ammoType) => setDraft({ ...draft, ammoType })} />
            <EnumSelect label="Range Category" value={draft.rangeCategory} options={WeaponRange} onChange={(rangeCategory) => setDraft({ ...draft, rangeCategory })} />
          </div>

          <EnumSelect label="Modifier Stat" value={draft.modifierStat} options={StatType} onChange={(modifierStat) => setDraft({ ...draft, modifierStat })} />

          <div className="grid grid-cols-2 gap-3">
            <Field label="Ammo Capacity">
              <input
                type="number"
                min="0"
                value={draft.ammoCapacity}
                onChange={(event) => setDraft({ ...draft, ammoCapacity: Number(event.target.value) })}
                className="field-input"
              />
            </Field>
            <Field label="Reload Time (Actions)">
              <input
                type="number"
                min="0"
                value={draft.reloadTimeInActions}
                onChange={(event) => setDraft({ ...draft, reloadTimeInActions: Number(event.target.value) })}
                className="field-input"
              />
            </Field>
          </div>

          <div className="grid grid-cols-3 gap-3">
            <Field label="Critical Range">
              <input
                type="number"
                min="1"
                max="20"
                value={draft.criticalRange}
                onChange={(event) => setDraft({ ...draft, criticalRange: Number(event.target.value) })}
                className="field-input"
              />
            </Field>
            <Field label="Critical Multiplier">
              <input
                type="number"
                min="1"
                value={draft.criticalMultiplier}
                onChange={(event) => setDraft({ ...draft, criticalMultiplier: Number(event.target.value) })}
                className="field-input"
              />
            </Field>
            <Field label="Critical Fail Range">
              <input
                type="number"
                min="1"
                max="20"
                value={draft.criticalFailRange ?? ''}
                onChange={(event) =>
                  setDraft({
                    ...draft,
                    criticalFailRange: event.target.value === '' ? null : Number(event.target.value),
                  })
                }
                placeholder="Optional"
                className="field-input"
              />
            </Field>
          </div>

          <button
            type="button"
            onClick={handleUpdateWeapon}
            disabled={!canUpdateWeapon || updateScifiWeapon.isPending}
            className="btn btn-accent"
          >
            {updateScifiWeapon.isPending ? 'Saving...' : 'Save Weapon Changes'}
          </button>
        </>
      )}

      {updateScifiWeapon.isError && (
        <p className="text-xs fg-danger">Failed to update weapon blueprint. Please try again.</p>
      )}
      {updateScifiWeapon.isSuccess && (
        <p className="text-xs fg-success">Weapon blueprint updated successfully.</p>
      )}
    </Card>
  )
}