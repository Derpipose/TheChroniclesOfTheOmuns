import NavBar from '../components/NavBar'
import { useDevSeeding } from '../hooks/useDevSeeding'
import type { SeedResult, SeedStatus } from '../types/devSeed'
import { useState } from 'react'
import { useCreateScifiWeapon } from '../hooks/useScifiWeapon'
import { StatType, DamageType,
    EnergyType, ScifiAmmoType, WeaponRange, } from '../types/scifiWeapon'

const statusColor = (s: SeedStatus) => {
  if (s === 'success') return '#4A7C59'
  if (s === 'error') return '#B04A2F'
  if (s === 'loading') return '#8B7355'
  return '#7A6F5C'
}

const statusLabel = (s: SeedStatus) => {
  if (s === 'success') return '✓ Done'
  if (s === 'error') return '✗ Failed'
  if (s === 'loading') return 'Loading...'
  return 'Not run'
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

  const handleCreateWeapon = () =>
    {
        if (!weaponName.trim() || !damageFormula.trim() || !rateOfFire.trim())
            { return }
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
                modifierStat, }, { onSuccess: () => { setWeaponName('')
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
                setModifierStat(StatType.Strength) }, } ) }

  const SeedButton = ({ label, onClick, table }: {
    label: string
    onClick: () => void
    table: keyof SeedResult
  }) => (
    <div
      className="rounded-xl border px-5 py-4 flex flex-col gap-1"
      style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
    >
      <div className="flex items-center justify-between">
        <div>
          <p className="font-semibold" style={{ color: '#2C2416' }}>{label}</p>
          <p className="text-xs mt-0.5" style={{ color: statusColor(status[table]) }}>
            {statusLabel(status[table])}
          </p>
        </div>
        <button
          onClick={onClick}
          disabled={status[table] === 'loading'}
          className="rounded-lg px-4 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50"
          style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
        >
          Seed
        </button>
      </div>
      {messages[table] && (
        <p className="text-xs break-words" style={{ color: '#7A6F5C' }}>
          {messages[table]}
        </p>
      )}
    </div>
  )

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
      <NavBar title="Dev Tools" />

      <div className="max-w-2xl mx-auto px-6 py-8">
        <div className="mb-6">
          <h2 className="text-xl font-semibold" style={{ color: '#2C2416' }}>Database Seeding</h2>
          <p className="text-sm mt-1" style={{ color: '#7A6F5C' }}>
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

        <div className="mt-6 rounded-xl border p-5 flex flex-col gap-4" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC', }} > <div> <h2 className="text-sm font-semibold uppercase tracking-wide" style={{ color: '#7A6F5C' }} > Add Weapon Blueprint </h2> <p className="text-xs mt-1" style={{ color: '#7A6F5C' }} > Create a weapon directly in the database for testing and seeding. </p> </div> {/* Name */} <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Name </label> <input type="text" value={weaponName} onChange={(e) => setWeaponName(e.target.value)} placeholder="e.g. Plasma Rifle" className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> {/* Damage Formula + Rate of Fire */} <div className="grid grid-cols-2 gap-3"> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Damage Formula </label> <input type="text" value={damageFormula} onChange={(e) => setDamageFormula(e.target.value)} placeholder="e.g. 2d6" className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Rate of Fire </label> <input type="text" value={rateOfFire} onChange={(e) => setRateOfFire(e.target.value)} placeholder="e.g. 1/Action" className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> </div> {/* Damage Type + Energy Type */} <div className="grid grid-cols-2 gap-3"> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Damage Type </label> <select value={damageType} onChange={(e) => setDamageType(e.target.value as DamageType) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} > {Object.values(DamageType).map((value) => ( <option key={value} value={value}> {value} </option> ))} </select> </div> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Energy Type </label> <select value={energyType} onChange={(e) => setEnergyType(e.target.value as EnergyType) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} > {Object.values(EnergyType).map((value) => ( <option key={value} value={value}> {value} </option> ))} </select> </div> </div> {/* Ammo Type + Range */} <div className="grid grid-cols-2 gap-3"> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Ammo Type </label> <select value={ammoType} onChange={(e) => setAmmoType(e.target.value as ScifiAmmoType) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} > {Object.values(ScifiAmmoType).map((value) => ( <option key={value} value={value}> {value} </option> ))} </select> </div> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Range Category </label> <select value={rangeCategory} onChange={(e) => setRangeCategory(e.target.value as WeaponRange) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} > {Object.values(WeaponRange).map((value) => ( <option key={value} value={value}> {value} </option> ))} </select> </div> </div> {/* Modifier Stat */} <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Modifier Stat </label> <select value={modifierStat} onChange={(e) => setModifierStat(e.target.value as StatType) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} > {Object.values(StatType).map((value) => ( <option key={value} value={value}> {value} </option> ))} </select> </div> {/* Numeric fields */} <div className="grid grid-cols-2 gap-3"> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Ammo Capacity </label> <input type="number" min="0" value={ammoCapacity} onChange={(e) => setAmmoCapacity(Number(e.target.value)) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Reload Time (Actions) </label> <input type="number" min="0" value={reloadTimeInActions} onChange={(e) => setReloadTimeInActions(Number(e.target.value)) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> </div> {/* Critical fields */} <div className="grid grid-cols-3 gap-3"> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Critical Range </label> <input type="number" min="1" max="20" value={criticalRange} onChange={(e) => setCriticalRange(Number(e.target.value)) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Critical Multiplier </label> <input type="number" min="1" value={criticalMultiplier} onChange={(e) => setCriticalMultiplier(Number(e.target.value)) } className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> <div className="flex flex-col gap-1"> <label className="text-xs font-medium" style={{ color: '#7A6F5C' }} > Critical Fail Range </label> <input type="number" min="1" max="20" value={criticalFailRange} onChange={(e) => setCriticalFailRange(e.target.value) } placeholder="Optional" className="rounded-lg border px-3 py-2 text-sm outline-none" style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416', }} /> </div> </div> {/* Submit */} <button onClick={handleCreateWeapon} disabled={ createScifiWeapon.isPending || !weaponName.trim() || !damageFormula.trim() || !rateOfFire.trim() } className="rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50" style={{ backgroundColor: '#8B7355', color: '#FDFAF4', }} > {createScifiWeapon.isPending ? 'Creating...' : 'Add Weapon to Database'} </button> {createScifiWeapon.isError && ( <p className="text-xs" style={{ color: '#B04A2F' }} > Failed to create weapon. </p> )} {createScifiWeapon.isSuccess && ( <p className="text-xs" style={{ color: '#4A7C59' }} > Weapon created successfully. </p> )} </div>
      </div>
    </div>
  )
}
