import { useState, useEffect,  } from 'react'
import { resolveExpression} from '../functions/DiceRoller'
import { CustomRollPresetCard, type CustomRollPreset } from '../components/CustomRollPresetCard'
import NavBar from '../components/NavBar'
import { useNavigate } from '@tanstack/react-router'
import { useCharacter, useCharacters, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useCharacterStats } from '../hooks/useCharacterStats'
import { useScifiCharacterWithStats } from '../hooks/useScifiCharacterWithStats'
import { getScifiCharacterByCharacterId } from '../api/scifiCharacterApi'
import {
  useScifiWeaponInstances,
  useUseAmmo,
  useReloadWeapon,
} from '../hooks/useScifiWeaponInstance'
import { useQuery } from '@tanstack/react-query'
interface DiceRoll {
  id: string
  timestamp: string
  expression: string
  resolvedExpr: string
  allRolls: number[]
  total: number
  label?: string
  attempts?: DiceRollAttempt[]
  rollMode?: RollMode
  chosenAttemptIndex?: number
}

type RollMode = 'standard' | 'advantage' | 'disadvantage'

interface DiceRollAttempt {
  resolvedExpr: string
  allRolls: number[]
  total: number
}

interface PendingWeaponRoll {
  weaponId: number
  criticalMultiplier: number
}


export default function DiceRollerPage() {
  const ROLL_DELAY_MS = 750
  const [activeTab, setActiveTab] = useState<'custom' | 'character'>('custom')
  const [rolls, setRolls] = useState<DiceRoll[]>([])
  const [selectedCharacterId, setSelectedCharacterId] = useState<number | ''>('')
  const [rollingStat, setRollingStat] = useState<string | null>(null)
  const [rollMode, setRollMode] = useState<RollMode>('standard')

  const { data: characters = [], isLoading: charactersLoading } = useCharacters()
  // TODO: Re-enable the active-only filter once the rest of the site supports inactive character flows more cleanly.
  const activeCharacters = characters

  const { data: selectedCharacter } = useCharacter(selectedCharacterId || 0)
  const { data: selectedScifiCharacter } = useScifiCharacterByCharacterId(selectedCharacterId || 0)
  const { data: selectedStats, isLoading: statsLoading } = useCharacterStats(selectedCharacterId || 0)

  const { data: scifiCharacterForCharacter } = useQuery({
    queryKey: ['scifiCharacterForCharacter', selectedCharacterId],
    queryFn: () => getScifiCharacterByCharacterId(selectedCharacterId || 0),
    enabled: !!selectedCharacterId,
  })

  // Debugging: inspect which scifiCharacter is resolved and adjusted result
  console.debug('scifiCharacterForCharacter', scifiCharacterForCharacter)

  const { data: selectedAdjustedScifi, isLoading: adjustedLoading } = useScifiCharacterWithStats(scifiCharacterForCharacter)

  console.debug('selectedAdjustedScifi', selectedAdjustedScifi)

  const effectiveStats = selectedAdjustedScifi ?? selectedStats

  const { data: selectedWeapons = [], isLoading: weaponsLoading } =
  useScifiWeaponInstances(selectedScifiCharacter?.id || 0)

  const sortedWeapons = [...selectedWeapons].sort((first, second) => {
    if (first.favorite !== second.favorite) {
      return first.favorite ? -1 : 1
    }

    return first.id - second.id
  })

    const fireAmmoMutation = useUseAmmo()

    const reloadWeaponMutation = useReloadWeapon()
    const [reloadingWeapon, setReloadingWeapon] = useState<number | null>(null)
    const [pendingWeaponRoll, setPendingWeaponRoll] = useState<PendingWeaponRoll | null>(null)

  useEffect(() => {
    if (activeCharacters.length === 0) {
      setSelectedCharacterId('')
      return
    }

    if (!selectedCharacterId || !activeCharacters.some((character) => character.id === selectedCharacterId)) {
      setSelectedCharacterId(activeCharacters[0].id)
    }
  }, [activeCharacters, selectedCharacterId])

  // Initialize from localStorage without an effect
  const [customPresets, setCustomPresets] = useState(() => {
    const saved = localStorage.getItem('diceroller_presets')
    if (saved) {
      try {
        return JSON.parse(saved)
      } catch (e) {
        console.error('Failed to load presets:', e)
        return []
      }
    }
    return []
  })

  const [newPresetName, setNewPresetName] = useState('')
  const [newPresetExpr, setNewPresetExpr] = useState('')
  const [showAddPreset, setShowAddPreset] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [rollNumber, setRollNumber] = useState(0)

  // Only keep the save effect (remove the load effect)
  useEffect(() => {
    localStorage.setItem('diceroller_presets', JSON.stringify(customPresets))
  }, [customPresets])

  const handleRoll = (
    expression: string,
    label?: string,
    rollKey?: string,
    onComplete?: (result: ReturnType<typeof resolveExpression>) => void,
    selectedRollMode: RollMode = 'standard',
  ) => {
    setError(null)
    if (selectedRollMode !== 'standard') {
      setRollMode('standard')
    }
    if (rollKey) {
      setRollingStat(rollKey)
    }

    window.setTimeout(() => {
      try {
        const attempts = selectedRollMode === 'standard'
          ? [resolveExpression(expression)]
          : [resolveExpression(expression), resolveExpression(expression)]
        const chosenAttemptIndex = selectedRollMode === 'advantage'
          ? (attempts[1].total > attempts[0].total ? 1 : 0)
          : selectedRollMode === 'disadvantage'
            ? (attempts[1].total < attempts[0].total ? 1 : 0)
            : 0
        const result = attempts[chosenAttemptIndex]
        const rollLabel = label && selectedRollMode !== 'standard'
          ? `${label} with ${selectedRollMode}`
          : label
        const roll: DiceRoll = {
          id: rollNumber.toString(),
          timestamp: new Date().toLocaleTimeString(),
          expression,
          resolvedExpr: result.resolvedExpr,
          allRolls: result.allRolls,
          total: result.total,
          label: rollLabel,
          attempts: selectedRollMode === 'standard' ? undefined : attempts,
          rollMode: selectedRollMode,
          chosenAttemptIndex,
        }
        setRolls((prev) => [roll, ...prev]) // append to bottom
        setRollNumber((prev) => prev + 1)
        onComplete?.(result)
      } catch (err) {
        setError((err as Error).message || 'Invalid dice expression')
      } finally {
        if (rollKey) {
          setRollingStat((current) => (current === rollKey ? null : current))
        }
      }
    }, ROLL_DELAY_MS)
  }

  const clearRolls = () => {
    setRolls([])
    setRollNumber(0)
  }

  const addPreset = () => {
    if (!newPresetName.trim() || !newPresetExpr.trim()) {
      setError('Name and expression are required')
      return
    }
    try {
      // Validate the expression by attempting to resolve it
      resolveExpression(newPresetExpr)
      const preset: CustomRollPreset = {
        id: Date.now().toString(),
        name: newPresetName,
        expression: newPresetExpr,
      }
      setCustomPresets((prev: CustomRollPreset[]) => [...prev, preset])
      setNewPresetName('')
      setNewPresetExpr('')
      setShowAddPreset(false)
      setError(null)
    } catch (err) {
      setError((err as Error).message || 'Invalid dice expression')
    }
  }

  const deletePreset = (id: string) => {
  setCustomPresets((prev: CustomRollPreset[]) => prev.filter((p) => p.id !== id))
}

  const statEntries = effectiveStats
    ? [
        { label: 'Strength', score: effectiveStats.strength },
        { label: 'Constitution', score: effectiveStats.constitution },
        { label: 'Dexterity', score: effectiveStats.dexterity },
        { label: 'Wisdom', score: effectiveStats.wisdom },
        { label: 'Charisma', score: effectiveStats.charisma },
        { label: 'Intelligence', score: effectiveStats.intelligence },
      ]
    : []

  const getAbilityModifier = (score: number) => Math.floor((score - 10) / 2)

  const getWeaponModifier = (weapon: typeof selectedWeapons[number]) => {
  if (!effectiveStats) return 0

  const statScores = {
    Strength: effectiveStats.strength,
    Constitution: effectiveStats.constitution,
    Dexterity: effectiveStats.dexterity,
    Wisdom: effectiveStats.wisdom,
    Charisma: effectiveStats.charisma,
    Intelligence: effectiveStats.intelligence,
  }

  const score = statScores[weapon.modifierStat]

  return getAbilityModifier(score)
}

  const statRows = statEntries.length ? Array.from({ length: Math.ceil(statEntries.length / 2) }, (_, index) =>
    statEntries.slice(index * 2, index * 2 + 2)
  ) : []

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
        <NavBar title="Dice Roller" />
      {/* Header */}
      <div
        className="border-b px-6 py-4 flex items-center justify-between"
        style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
      >
        <div className="flex items-center gap-2">
          <span className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
            Roll mode
          </span>
          {(['standard', 'advantage', 'disadvantage'] as RollMode[]).map((mode) => (
            <label
              key={mode}
              className="flex items-center gap-1 rounded border px-2 py-2 text-xs capitalize cursor-pointer"
              style={{
                backgroundColor: rollMode === mode ? '#EDE8DC' : '#FDFAF4',
                borderColor: '#DDD8CC',
                color: '#5C4F38',
              }}
            >
              <input
                type="radio"
                name="roll-mode"
                value={mode}
                checked={rollMode === mode}
                onChange={() => setRollMode(mode)}
              />
              {mode}
            </label>
          ))}
        </div>

        <button
          onClick={clearRolls}
          className="px-4 py-2 text-sm rounded transition-colors hover:opacity-90 disabled:opacity-50"
          style={{
            backgroundColor: '#8B7355',
            color: '#FDFAF4',
          }}
          disabled={rolls.length === 0 || rollingStat !== null}
        >
          Clear Log
        </button>
      </div>

      {/* Main Content */}
      <div className="flex h-[calc(100vh-80px)]">
        {/* Left Panel - Roll Inputs */}
        <div
          className="w-1/3 border-r overflow-y-auto p-6 flex flex-col gap-4"
          style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC' }}
        >
          {/* Tab Switcher */}
          <div className="flex gap-2 mb-4">
            <button
              onClick={() => setActiveTab('custom')}
              className="flex-1 py-2 px-3 text-sm rounded transition-all border"
              style={{
                backgroundColor: activeTab === 'custom' ? '#8B7355' : '#EDE8DC',
                color: activeTab === 'custom' ? '#FDFAF4' : '#5C4F38',
                borderColor: '#DDD8CC',
              }}
            >
              Custom
            </button>
            <button
              onClick={() => setActiveTab('character')}
              className="flex-1 py-2 px-3 text-sm rounded transition-all border"
              style={{
                backgroundColor: activeTab === 'character' ? '#8B7355' : '#EDE8DC',
                color: activeTab === 'character' ? '#FDFAF4' : '#5C4F38',
                borderColor: '#DDD8CC',
              }}
            >
              Character
            </button>
          </div>

          {/* Custom Rolls Tab */}
          {activeTab === 'custom' && (
            <div className="space-y-4">
              <div className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                Custom Presets
              </div>

              {customPresets.length === 0 && !showAddPreset && (
                <p className="text-xs text-center" style={{ color: '#7A6F5C' }}>
                  No presets yet
                </p>
              )}

              {customPresets.map((preset: CustomRollPreset) => (
                <CustomRollPresetCard
                  key={preset.id}
                  preset={preset}
                  onRoll={handleRoll}
                  onDelete={deletePreset}
                  isRolling={rollingStat === `preset-${preset.id}`}
                />
              ))}

              {showAddPreset ? (
                <div
                  className="rounded-lg border p-3 space-y-3"
                  style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
                >
                  <input
                    type="text"
                    placeholder="Preset name"
                    value={newPresetName}
                    onChange={(e) => setNewPresetName(e.target.value)}
                    className="w-full px-3 py-2 text-sm rounded outline-none border focus:border-stone-400 transition-colors"
                    style={{
                      backgroundColor: '#F5F0E8',
                      borderColor: '#DDD8CC',
                      color: '#2C2416',
                    }}
                  />
                  <input
                    type="text"
                    placeholder="e.g., 1d20 + 3"
                    value={newPresetExpr}
                    onChange={(e) => setNewPresetExpr(e.target.value)}
                    className="w-full px-3 py-2 text-sm rounded outline-none border focus:border-stone-400 transition-colors font-mono"
                    style={{
                      backgroundColor: '#F5F0E8',
                      borderColor: '#DDD8CC',
                      color: '#2C2416',
                    }}
                  />
                  <div className="flex gap-2">
                    <button
                      onClick={addPreset}
                      className="flex-1 py-2 text-xs font-semibold rounded transition-colors hover:opacity-90"
                      style={{
                        backgroundColor: '#8B7355',
                        color: '#FDFAF4',
                      }}
                    >
                      Add
                    </button>
                    <button
                      onClick={() => {
                        setShowAddPreset(false)
                        setNewPresetName('')
                        setNewPresetExpr('')
                      }}
                      className="flex-1 py-2 text-xs font-medium rounded transition-colors"
                      style={{
                        backgroundColor: '#EDE8DC',
                        color: '#5C4F38',
                      }}
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              ) : (
                <button
                  onClick={() => setShowAddPreset(true)}
                  className="w-full py-2 text-xs font-medium rounded transition-colors border"
                  style={{
                    backgroundColor: '#F5F0E8',
                    borderColor: '#DDD8CC',
                    color: '#5C4F38',
                  }}
                >
                  + New Preset
                </button>
              )}
            </div>
          )}

          {/* Character Rolls Tab */}
          {activeTab === 'character' && (
            <div className="space-y-4">
              <div className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                Character Rolls
              </div>

              {charactersLoading ? (
                <p className="text-sm" style={{ color: '#7A6F5C' }}>
                  Loading characters...
                </p>
              ) : activeCharacters.length === 0 ? (
                <div
                  className="rounded-lg border p-4 text-center"
                  style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
                >
                  <p className="text-sm font-medium" style={{ color: '#2C2416' }}>
                    No active characters available.
                  </p>
                </div>
              ) : (
                <>
                  <label className="flex flex-col gap-2">
                    <span className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                      Select Character
                    </span>
                    <select
                      value={selectedCharacterId}
                      onChange={(event) => setSelectedCharacterId(Number(event.target.value))}
                      className="w-full rounded-lg border px-3 py-2 text-sm outline-none transition-colors"
                      style={{
                        backgroundColor: '#FDFAF4',
                        borderColor: '#DDD8CC',
                        color: '#2C2416',
                      }}
                    >
                      {activeCharacters.map((character) => (
                        <option key={character.id} value={character.id}>
                          {character.name}
                        </option>
                      ))}
                    </select>
                  </label>

                  {selectedCharacter && (
                    <p className="text-xs" style={{ color: '#7A6F5C' }}>
                      Selected: <span className="font-semibold" style={{ color: '#2C2416' }}>{selectedCharacter.name}</span>
                    </p>
                  )}

                  {statsLoading || adjustedLoading ? (
                    <p className="text-sm" style={{ color: '#7A6F5C' }}>
                      Loading character stats...
                    </p>
                  ) : effectiveStats ? (
                    <div className="space-y-4">
                      <h2 className="text-sm font-semibold uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                        Character Stats
                      </h2>

                      <div className="space-y-3">
                        {statRows.map((row, rowIndex) => (
                          <div key={`stat-row-${rowIndex}`} className="grid grid-cols-2 gap-2">
                            {row.map(({ label, score }) => {
                              const modifier = getAbilityModifier(score)

                              return (
                                <div
                                  key={label}
                                  className="rounded-lg border p-3 space-y-2"
                                  style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
                                >
                                  <div className="flex items-center justify-between gap-2">
                                    <div className="flex-1 min-w-0">
                                      <p className="text-sm font-semibold" style={{ color: '#2C2416' }}>
                                        {label}
                                      </p>
                                      <p className="text-xs mt-1" style={{ color: '#7A6F5C' }}>
                                        Score: {score} | Mod: {modifier >= 0 ? `+${modifier}` : modifier}
                                      </p>
                                    </div>
                                  </div>
                                  <button
                                    type="button"
                                    onClick={() => handleRoll(
                                      `1d20 + ${modifier}`,
                                      `${label} check`,
                                      label,
                                      undefined,
                                      rollMode,
                                    )}
                                    disabled={rollingStat === label}
                                    className="w-full py-2 text-xs font-semibold rounded transition-colors hover:opacity-90 disabled:opacity-60 disabled:cursor-not-allowed"
                                    style={{
                                      backgroundColor: '#8B7355',
                                      color: '#FDFAF4',
                                    }}
                                  >
                                    {rollingStat === label ? 'Rolling' : 'Roll'}
                                  </button>
                                </div>
                              )
                            })}
                          </div>
                        ))}
                      </div>

                      <div className="pt-2">
  <h2
    className="text-sm font-semibold uppercase tracking-wide"
    style={{ color: '#7A6F5C' }}
  >
    Weapons
  </h2>

  {weaponsLoading ? (
    <p className="text-sm mt-2" style={{ color: '#7A6F5C' }}>
      Loading weapons...
    </p>
  ) : selectedWeapons.length === 0 ? (
    <div
      className="rounded-lg border p-4 mt-2 text-center"
      style={{
        backgroundColor: '#FDFAF4',
        borderColor: '#DDD8CC',
      }}
    >
      <p className="text-sm font-medium" style={{ color: '#2C2416' }}>
        This character has no weapons.
      </p>
    </div>
  ) : (
    <div className="space-y-3 mt-2">
      {sortedWeapons.map((weapon) => {
        // Determine ability modifier from effective (possibly adjusted) stats

        const handleWeaponReload = () => {
            setReloadingWeapon(weapon.id)

            reloadWeaponMutation.mutate(weapon.id, {
                onSettled: () => {
                setReloadingWeapon((current) =>
            current === weapon.id ? null : current
            )
            },
            })
        }
        const modifier = getWeaponModifier(weapon)

        const isReloading = reloadingWeapon === weapon.id
        const isFullAmmo = weapon.currentAmmo >= weapon.ammoCapacity

        // ===== AMMO & DAMAGE FORMULA CALCULATION =====
        // Parse rateOfFire from string to number
        const rateOfFire = parseInt(weapon.rateOfFire, 10) || 1

        // Calculate how many rounds actually fire (limited by current ammo)
        const ammoToConsume = Math.min(rateOfFire, weapon.currentAmmo)

        // Roll ROF first, then apply it to every die in the weapon formula.
        const damageFormula = `1d${ammoToConsume} * ${weapon.damageFormula}`
        const modifierExpression = `${modifier >= 0 ? '+' : '-'} ${Math.abs(modifier)}`
        const weaponRollKey = `weapon-${weapon.id}`
        const isOutOfAmmo = weapon.currentAmmo === 0
        const isFiring = pendingWeaponRoll?.weaponId === weapon.id

        const consumeWeaponAmmo = () => {
          if (ammoToConsume > 0) {
            fireAmmoMutation.mutate({ id: weapon.id, amount: ammoToConsume })
          }
          setPendingWeaponRoll(null)
        }

        const handleWeaponHitRoll = () => {
          const hitExpression = `1d20 ${modifierExpression}`

          handleRoll(
            hitExpression,
            `${weapon.nickname || weapon.name} to hit`,
            weaponRollKey,
            (result) => {
              const d20Result = result.allRolls[0]

              setPendingWeaponRoll({
                weaponId: weapon.id,
                criticalMultiplier:
                  d20Result >= weapon.criticalRange
                    ? weapon.criticalMultiplier
                    : 1,
              })
            },
            rollMode,
          )
        }

        const handleWeaponDamage = () => {
          const criticalMultiplier = pendingWeaponRoll?.criticalMultiplier ?? 1
          const expression = criticalMultiplier === 1
            ? `${damageFormula} ${modifierExpression}`
            : `${criticalMultiplier} * ${damageFormula} ${modifierExpression}`

          handleRoll(
            expression,
            `${weapon.nickname || weapon.name} damage`,
            weaponRollKey,
            consumeWeaponAmmo,
          )
        }

        return (
          <div
            key={weapon.id}
            className="rounded-lg border p-3 space-y-2 transition-opacity"
            style={{
                backgroundColor: isOutOfAmmo ? '#F3E2DE' : '#FDFAF4',
                borderColor: isOutOfAmmo ? '#C98A7D' : '#DDD8CC',
                opacity: 1,
            }}
          >
            <div className="flex items-center justify-between gap-2">
              <div className="flex-1 min-w-0">
                <p
                  className="text-sm font-semibold"
                  style={{ color: '#2C2416' }}
                >
                  {weapon.nickname || weapon.name}
                </p>

                <p
                  className="text-xs mt-1 font-mono"
                  style={{ color: '#7A6F5C' }}
                >
                  {damageFormula} {modifierExpression}
                </p>

                <p
                  className="text-xs mt-1"
                  style={{ color: '#7A6F5C' }}
                >
                  ROF: {rateOfFire}
                  {' || '}
                  <span className={isOutOfAmmo ? 'font-semibold' : ''}>
                    Ammo: {weapon.currentAmmo}/{weapon.ammoCapacity}
                  </span>
                </p>
              </div>

              {weapon.favorite && (
                <span
                  className="text-xs"
                  title="Favorite"
                  style={{ color: '#8B7355' }}
                >
                  ★
                </span>
              )}
            </div>

            <div className="flex gap-2">
  <button
    type="button"
    onClick={isFiring ? handleWeaponDamage : handleWeaponHitRoll}
    disabled={rollingStat === weaponRollKey || isOutOfAmmo || isReloading}
    className="flex-1 py-2 text-xs font-semibold rounded transition-all hover:opacity-90 disabled:opacity-60 disabled:cursor-not-allowed"
    style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
  >
    {isOutOfAmmo
      ? 'Out of Ammo'
      : rollingStat === weaponRollKey
        ? 'Rolling...'
        : isFiring
          ? 'Roll damage'
          : 'Roll to hit'}
  </button>

  <button
    type="button"
    onClick={isFiring ? consumeWeaponAmmo : handleWeaponReload}
    disabled={
      isReloading ||
      rollingStat === weaponRollKey ||
      (!isFiring && isFullAmmo) ||
      (isFiring && ammoToConsume <= 0)
    }
    className="flex-1 py-2 text-xs font-semibold rounded transition-all hover:opacity-90 disabled:opacity-60 disabled:cursor-not-allowed"
    style={{
      backgroundColor: isFiring ? '#EDE8DC' : isOutOfAmmo ? '#A85C4B' : '#EDE8DC',
      color: !isFiring && isOutOfAmmo ? '#FDFAF4' : '#5C4F38',
      border: !isFiring && isOutOfAmmo ? '1px solid #8F493C' : '1px solid #DDD8CC',
    }}
  >
    {isFiring
      ? 'Cancel'
      : isReloading
        ? 'Reloading...'
        : `Reload ${weapon.reloadTimeInActions} actions`}
  </button>
</div>
          </div>
        )
      })}
    </div>
  )}
</div>
                    </div>
                  ) : (
                    <div
                      className="rounded-lg border p-4 text-center"
                      style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
                    >
                      <p className="text-sm font-medium" style={{ color: '#2C2416' }}>
                        No stats found for this character.
                      </p>
                    </div>
                  )}
                </>
              )}
            </div>
          )}

          {/* Error Display */}
          {error && (
            <div
              className="rounded-lg p-3 text-sm"
              style={{ backgroundColor: '#F5F0E8', color: '#A85C4B', border: '1px solid #DDD8CC' }}
            >
              {error}
            </div>
          )}
        </div>

        {/* Right Panel - Roll Log */}
        <div
          className="w-2/3 overflow-y-auto p-6"
          style={{ backgroundColor: '#F5F0E8' }}
        >
          <div className="space-y-3">
            {rolls.length === 0 ? (
              <div className="text-center py-12">
                <p
                  className="text-sm"
                  style={{ color: '#7A6F5C' }}
                >
                  No rolls yet
                </p>
              </div>
            ) : (
              rolls.map((roll) => (
                <div
                  key={roll.id}
                  className="rounded-lg border p-4"
                  style={{
                    backgroundColor: '#FDFAF4',
                    borderColor: '#DDD8CC',
                  }}
                >
                  <div className="flex items-start justify-between gap-4 mb-2">
                    <div className="flex-1 min-w-0">
                      {roll.label && (
                        <p
                          className="text-sm font-semibold mb-1"
                          style={{ color: '#2C2416' }}
                        >
                          {roll.label}
                        </p>
                      )}
                      <p
                        className="text-xs"
                        style={{ color: '#7A6F5C' }}
                      >
                        {roll.timestamp}
                      </p>
                    </div>
                    <div className="text-right">
                      {roll.attempts ? (
                        <div className="flex gap-2">
                          {roll.attempts.map((attempt, index) => (
                            <div
                              key={`${roll.id}-summary-${index}`}
                              className="rounded border px-2 py-1"
                              style={{
                                borderColor: index === roll.chosenAttemptIndex ? '#8B7355' : '#DDD8CC',
                                backgroundColor: index === roll.chosenAttemptIndex ? '#F3EBDD' : '#FDFAF4',
                              }}
                            >
                              <p className="text-2xl font-bold font-mono" style={{ color: '#8B7355' }}>
                                {attempt.total}
                              </p>
                              <p className="text-xs" style={{ color: '#7A6F5C' }}>
                                {index === roll.chosenAttemptIndex ? 'Chosen' : `Roll ${index + 1}`}
                              </p>
                            </div>
                          ))}
                        </div>
                      ) : (
                        <p className="text-2xl font-bold font-mono" style={{ color: '#8B7355' }}>
                          {roll.total}
                        </p>
                      )}
                    </div>
                  </div>

                  <div className="space-y-1 pt-2 border-t" style={{ borderColor: '#DDD8CC' }}>
                    <p
                      className="text-xs font-mono"
                      style={{ color: '#2C2416' }}
                    >
                      {roll.expression}
                    </p>
                    {roll.attempts && (
                      <div className="space-y-2 pt-1">
                        <p className="text-xs capitalize" style={{ color: '#7A6F5C' }}>
                          {roll.rollMode}
                        </p>
                        {roll.attempts.map((attempt, index) => (
                          <div
                            key={`${roll.id}-attempt-${index}`}
                            className="rounded border px-2 py-1"
                            style={{
                              borderColor: index === roll.chosenAttemptIndex ? '#8B7355' : '#DDD8CC',
                              backgroundColor: index === roll.chosenAttemptIndex ? '#F3EBDD' : '#FDFAF4',
                            }}
                          >
                            <p className="text-xs font-mono" style={{ color: '#2C2416' }}>
                              Attempt {index + 1}: {attempt.total}
                              {index === roll.chosenAttemptIndex && ' — Chosen'}
                            </p>
                            <p className="text-xs font-mono" style={{ color: '#7A6F5C' }}>
                              [{attempt.allRolls.join(', ')}]
                            </p>
                          </div>
                        ))}
                      </div>
                    )}
                    <p
                      className="text-xs font-mono"
                      style={{ color: '#7A6F5C' }}
                    >
                      = {roll.resolvedExpr}
                    </p>
                    <p
                      className="text-xs font-mono break-all"
                      style={{ color: '#7A6F5C' }}
                    >
                      [{roll.allRolls.join(', ')}]
                    </p>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
