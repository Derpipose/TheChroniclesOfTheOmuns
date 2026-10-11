import { useState } from 'react'
import Card from '../Card'
import { useCreateScifiWeapon } from '../../hooks/useScifiWeapon'
import { StatType, DamageType, EnergyType, ScifiAmmoType, WeaponRange } from '../../types/scifiWeapon'
import EnumSelect from './EnumSelect'
import Field from './Field'

/** Form for creating a weapon blueprint directly in the database. */
export default function AddWeaponCard() {
  const createScifiWeapon = useCreateScifiWeapon()

  const [weaponName, setWeaponName] = useState('')
  const [damageFormula, setDamageFormula] = useState('')
  const [ammoType, setAmmoType] = useState<ScifiAmmoType>(ScifiAmmoType.None)
  const [damageType, setDamageType] = useState<DamageType>(DamageType.Kinetic)
  const [energyType, setEnergyType] = useState<EnergyType>(EnergyType.Physical)
  const [rateOfFire, setRateOfFire] = useState('')
  const [ammoCapacity, setAmmoCapacity] = useState(0)
  const [criticalRange, setCriticalRange] = useState(20)
  const [criticalMultiplier, setCriticalMultiplier] = useState(2)
  const [reloadTimeInActions, setReloadTimeInActions] = useState(0)
  const [rangeCategory, setRangeCategory] = useState<WeaponRange>(WeaponRange.None)
  const [criticalFailRange, setCriticalFailRange] = useState('')
  const [modifierStat, setModifierStat] = useState<StatType>(StatType.Strength)

  const canCreateWeapon =
    weaponName.trim() !== '' && damageFormula.trim() !== '' && rateOfFire.trim() !== ''

  const resetWeaponForm = () => {
    setWeaponName('')
    setDamageFormula('')
    setAmmoType(ScifiAmmoType.None)
    setDamageType(DamageType.Kinetic)
    setEnergyType(EnergyType.Physical)
    setRateOfFire('')
    setAmmoCapacity(0)
    setCriticalRange(20)
    setCriticalMultiplier(2)
    setReloadTimeInActions(0)
    setRangeCategory(WeaponRange.None)
    setCriticalFailRange('')
    setModifierStat(StatType.Strength)
  }

  const handleCreateWeapon = () => {
    if (!canCreateWeapon) return

    createScifiWeapon.mutate(
      {
        name: weaponName.trim(),
        damageFormula: damageFormula.trim(),
        ammoType,
        damageType,
        energyType,
        rateOfFire: rateOfFire.trim(),
        ammoCapacity,
        criticalRange,
        criticalMultiplier,
        reloadTimeInActions,
        rangeCategory,
        criticalFailRange: criticalFailRange.trim() === '' ? undefined : Number(criticalFailRange),
        modifierStat,
      },
      { onSuccess: resetWeaponForm }
    )
  }

  return (
    <Card className="flex flex-col gap-4" title="Add Weapon Blueprint" description="Create a weapon directly in the database for testing and seeding.">
      <Field label="Name">
        <input
          type="text"
          value={weaponName}
          onChange={(e) => setWeaponName(e.target.value)}
          placeholder="e.g. Plasma Rifle"
          className="field-input"
        />
      </Field>

      <div className="grid grid-cols-2 gap-3">
        <Field label="Damage Formula">
          <input
            type="text"
            value={damageFormula}
            onChange={(e) => setDamageFormula(e.target.value)}
            placeholder="e.g. 2d6"
            className="field-input"
          />
        </Field>
        <Field label="Rate of Fire">
          <input
            type="text"
            value={rateOfFire}
            onChange={(e) => setRateOfFire(e.target.value)}
            placeholder="e.g. 1/Action"
            className="field-input"
          />
        </Field>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <EnumSelect label="Damage Type" value={damageType} options={DamageType} onChange={setDamageType} />
        <EnumSelect label="Energy Type" value={energyType} options={EnergyType} onChange={setEnergyType} />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <EnumSelect label="Ammo Type" value={ammoType} options={ScifiAmmoType} onChange={setAmmoType} />
        <EnumSelect label="Range Category" value={rangeCategory} options={WeaponRange} onChange={setRangeCategory} />
      </div>

      <EnumSelect label="Modifier Stat" value={modifierStat} options={StatType} onChange={setModifierStat} />

      <div className="grid grid-cols-2 gap-3">
        <Field label="Ammo Capacity">
          <input
            type="number"
            min="0"
            value={ammoCapacity}
            onChange={(e) => setAmmoCapacity(Number(e.target.value))}
            className="field-input"
          />
        </Field>
        <Field label="Reload Time (Actions)">
          <input
            type="number"
            min="0"
            value={reloadTimeInActions}
            onChange={(e) => setReloadTimeInActions(Number(e.target.value))}
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
            value={criticalRange}
            onChange={(e) => setCriticalRange(Number(e.target.value))}
            className="field-input"
          />
        </Field>
        <Field label="Critical Multiplier">
          <input
            type="number"
            min="1"
            value={criticalMultiplier}
            onChange={(e) => setCriticalMultiplier(Number(e.target.value))}
            className="field-input"
          />
        </Field>
        <Field label="Critical Fail Range">
          <input
            type="number"
            min="1"
            max="20"
            value={criticalFailRange}
            onChange={(e) => setCriticalFailRange(e.target.value)}
            placeholder="Optional"
            className="field-input"
          />
        </Field>
      </div>

      <button
        onClick={handleCreateWeapon}
        disabled={createScifiWeapon.isPending || !canCreateWeapon}
        className="btn btn-accent"
      >
        {createScifiWeapon.isPending ? 'Creating...' : 'Add Weapon to Database'}
      </button>

      {createScifiWeapon.isError && (
        <p className="text-xs fg-danger">Failed to create weapon.</p>
      )}
      {createScifiWeapon.isSuccess && (
        <p className="text-xs fg-success">Weapon created successfully.</p>
      )}
    </Card>
  )
}
