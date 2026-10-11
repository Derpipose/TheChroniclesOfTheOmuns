import NavBar from '../components/NavBar'
import { useDevSeeding } from '../hooks/useDevSeeding'
import type { SeedResult, SeedStatus } from '../types/devSeed'
import { useState, type ReactNode } from 'react'
import {
  useCreateScifiWeapon,
  useDeleteScifiWeapon,
  useScifiWeapons,
} from '../hooks/useScifiWeapon'
import { StatType, DamageType, EnergyType, ScifiAmmoType, WeaponRange, } from '../types/scifiWeapon'

const statusColor = (s: SeedStatus) => {
  if (s === 'success') return '#4A7C59'
  if (s === 'error') return 'var(--color-secondary)'
  if (s === 'loading') return 'var(--color-accent)'
  return 'var(--color-text-60)'
}

const statusLabel = (s: SeedStatus) => {
  if (s === 'success') return '✓ Done'
  if (s === 'error') return '✗ Failed'
  if (s === 'loading') return 'Loading...'
  return 'Not run'
}

const controlStyle = {
  backgroundColor: 'var(--color-background)',
  borderColor: 'var(--color-primary-30)',
  color: 'var(--color-text)',
}

function Card({ title, description, children }: {
  title: string
  description?: string
  children: ReactNode
}) {
  return (
    <div
      className="mt-6 rounded-xl border p-5 flex flex-col gap-4"
      style={{ backgroundColor: 'var(--color-background-30)', borderColor: 'var(--color-primary-30)' }}
    >
      <div>
        <h2 className="text-sm font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>
          {title}
        </h2>
        {description && (
          <p className="text-xs mt-1" style={{ color: 'var(--color-text-60)' }}>{description}</p>
        )}
      </div>
      {children}
    </div>
  )
}

function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className="flex flex-col gap-1">
      <label className="text-xs font-medium" style={{ color: 'var(--color-text-60)' }}>{label}</label>
      {children}
    </div>
  )
}

function EnumSelect<T extends string>({ label, value, options, onChange }: {
  label: string
  value: T
  options: Record<string, T>
  onChange: (value: T) => void
}) {
  return (
    <Field label={label}>
      <select
        value={value}
        onChange={(event) => onChange(event.target.value as T)}
        className="field-input"
        style={controlStyle}
      >
        {Object.values(options).map((option) => (
          <option key={option} value={option}>{option}</option>
        ))}
      </select>
    </Field>
  )
}

export default function DevPage() {
  const {
    status,
    messages,
    seedTalents,
    seedClasses,
    seedBackgrounds,
    seedRaces,
    seedSubraces,
    seedWeapons,
  } = useDevSeeding()

  const createScifiWeapon = useCreateScifiWeapon()
  const deleteScifiWeapon = useDeleteScifiWeapon()
  const { data: scifiWeapons = [] } = useScifiWeapons()

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

  const SeedButton = ({ label, onClick, table }: {
    label: string
    onClick: () => void
    table: keyof SeedResult
  }) => (
    <div
      className="rounded-xl border px-5 py-4 flex flex-col gap-1"
      style={{ backgroundColor: 'var(--color-background-30)', borderColor: 'var(--color-primary-30)' }}
    >
      <div className="flex items-center justify-between">
        <div>
          <p className="font-semibold" style={{ color: 'var(--color-text)' }}>{label}</p>
          <p className="text-xs mt-0.5" style={{ color: statusColor(status[table]) }}>
            {statusLabel(status[table])}
          </p>
        </div>
        <button
          onClick={onClick}
          disabled={status[table] === 'loading'}
          className="btn px-4"
          style={{ backgroundColor: 'var(--color-accent)', color: 'var(--color-text-30)' }}
        >
          Seed
        </button>
      </div>
      {messages[table] && (
        <p className="text-xs break-words" style={{ color: 'var(--color-text-60)' }}>
          {messages[table]}
        </p>
      )}
    </div>
  )

  return (
    <div className="min-h-screen" style={{ backgroundColor: 'var(--color-background)' }}>
      <NavBar title="Dev Tools" />

      <div className="max-w-2xl mx-auto px-6 py-8">
        <div className="mb-6">
          <h2 className="text-xl font-semibold" style={{ color: 'var(--color-text)' }}>Database Seeding</h2>
          <p className="text-sm mt-1" style={{ color: 'var(--color-text-60)' }}>
            Seed tables in order — talents must be seeded before classes and backgrounds.
          </p>
        </div>

        <div className="flex flex-col gap-3">
          <SeedButton label="1. Seed Talents" onClick={seedTalents} table="talents" />
          <SeedButton label="2. Seed Classes" onClick={seedClasses} table="classes" />
          <SeedButton label="3. Seed Backgrounds" onClick={seedBackgrounds} table="backgrounds" />
          <SeedButton label="4. Seed Races" onClick={seedRaces} table="races" />
          <SeedButton label="5. Seed Subraces" onClick={seedSubraces} table="subraces" />
          <SeedButton label="6. Seed Weapons" onClick={seedWeapons} table="weapons" />
        </div>

        <Card title="Add Weapon Blueprint" description="Create a weapon directly in the database for testing and seeding.">
          <Field label="Name">
            <input
              type="text"
              value={weaponName}
              onChange={(e) => setWeaponName(e.target.value)}
              placeholder="e.g. Plasma Rifle"
              className="field-input"
              style={controlStyle}
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
                style={controlStyle}
              />
            </Field>
            <Field label="Rate of Fire">
              <input
                type="text"
                value={rateOfFire}
                onChange={(e) => setRateOfFire(e.target.value)}
                placeholder="e.g. 1/Action"
                className="field-input"
                style={controlStyle}
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
                style={controlStyle}
              />
            </Field>
            <Field label="Reload Time (Actions)">
              <input
                type="number"
                min="0"
                value={reloadTimeInActions}
                onChange={(e) => setReloadTimeInActions(Number(e.target.value))}
                className="field-input"
                style={controlStyle}
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
                style={controlStyle}
              />
            </Field>
            <Field label="Critical Multiplier">
              <input
                type="number"
                min="1"
                value={criticalMultiplier}
                onChange={(e) => setCriticalMultiplier(Number(e.target.value))}
                className="field-input"
                style={controlStyle}
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
                style={controlStyle}
              />
            </Field>
          </div>

          <button
            onClick={handleCreateWeapon}
            disabled={createScifiWeapon.isPending || !canCreateWeapon}
            className="btn"
            style={{ backgroundColor: 'var(--color-accent)', color: 'var(--color-text-30)' }}
          >
            {createScifiWeapon.isPending ? 'Creating...' : 'Add Weapon to Database'}
          </button>

          {createScifiWeapon.isError && (
            <p className="text-xs" style={{ color: 'var(--color-secondary)' }}>Failed to create weapon.</p>
          )}
          {createScifiWeapon.isSuccess && (
            <p className="text-xs" style={{ color: '#4A7C59' }}>Weapon created successfully.</p>
          )}
        </Card>

        <Card
          title="Delete Weapon Blueprint"
          description="Search for a blueprint, select it, and remove it from the database."
        >
          <input
            type="text"
            value={weaponSearch}
            onChange={(event) => setWeaponSearch(event.target.value)}
            placeholder="Search weapon blueprints"
            className="field-input"
            style={controlStyle}
          />

          <select
            value={selectedWeaponId}
            onChange={(event) =>
              setSelectedWeaponId(event.target.value ? Number(event.target.value) : '')
            }
            className="field-input"
            style={controlStyle}
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
            className="btn border"
            style={{
              backgroundColor: 'var(--color-secondary-30)',
              borderColor: 'var(--color-secondary-60)',
              color: 'var(--color-secondary)',
            }}
          >
            {deleteScifiWeapon.isPending ? 'Deleting...' : 'Delete Blueprint'}
          </button>

          {deleteScifiWeapon.isError && (
            <p className="text-xs" style={{ color: 'var(--color-secondary)' }}>
              Failed to delete weapon blueprint. Please try again.
            </p>
          )}
        </Card>
      </div>
    </div>
  )
}
