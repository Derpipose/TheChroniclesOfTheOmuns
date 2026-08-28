import { useState, useEffect,  } from 'react'
import { resolveExpression} from '../functions/DiceRoller'
import CustomRolls from '../components/CustomRolls'
import RollLog, { type DiceRoll, type RollMode } from '../components/RollLog'
import NavBar from '../components/NavBar'
import { useCharacter, useCharacters, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useCharacterStats } from '../hooks/useCharacterStats'
import { useScifiCharacterWithStats } from '../hooks/useScifiCharacterWithStats'
import {
  useScifiWeaponInstances,
  useUseAmmo,
  useReloadWeapon,
} from '../hooks/useScifiWeaponInstance'
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
  const [rollLogVisible, setRollLogVisible] = useState(true)
  const [rollAlert, setRollAlert] = useState<string | null>(null)

  const { data: characters = [], isLoading: charactersLoading } = useCharacters()
  // TODO: Re-enable the active-only filter once the rest of the site supports inactive character flows more cleanly.
  const activeCharacters = characters

  const { data: selectedCharacter } = useCharacter(selectedCharacterId || 0)
  const { data: selectedScifiCharacter } = useScifiCharacterByCharacterId(selectedCharacterId || 0)
  const { data: selectedStats, isLoading: statsLoading } = useCharacterStats(selectedCharacterId || 0)


  const { data: selectedAdjustedScifi, isLoading: adjustedLoading } = useScifiCharacterWithStats(selectedScifiCharacter?.id)

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

  const [error, setError] = useState<string | null>(null)
  const [rollNumber, setRollNumber] = useState(0)

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
        if (!rollLogVisible) {
          setRollAlert(`${roll.label ?? roll.expression}: ${roll.total}`)
        }
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

  const handleRollLogVisibilityChange = (isVisible: boolean) => {
    setRollLogVisible(isVisible)
    if (isVisible) {
      setRollAlert(null)
    }
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
        className="sticky top-0 z-20 border-b px-4 py-3 shadow-sm sm:px-6 sm:py-4 flex flex-col items-stretch gap-3 sm:flex-row sm:items-center sm:justify-between"
        style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
      >
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
            Roll mode
          </span>
          {(['standard', 'advantage', 'disadvantage'] as RollMode[]).map((mode) => (
            <label
              key={mode}
                className="flex items-center gap-1 rounded border px-1.5 py-1.5 text-[11px] capitalize cursor-pointer sm:px-2 sm:py-2 sm:text-xs"
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
          className="w-full px-3 py-1.5 text-xs rounded transition-colors hover:opacity-90 disabled:opacity-50 sm:w-auto sm:px-4 sm:py-2 sm:text-sm"
          style={{
            backgroundColor: '#8B7355',
            color: '#FDFAF4',
          }}
          disabled={rolls.length === 0 || rollingStat !== null}
        >
          Clear Log
        </button>
      </div>

      {rollAlert && (
        <div
          className="fixed inset-x-4 top-24 z-30 flex items-center justify-between gap-3 rounded-lg border px-3 py-2 shadow-lg sm:inset-x-auto sm:right-6 sm:w-auto sm:min-w-72"
          style={{ backgroundColor: '#FDFAF4', borderColor: '#8B7355', color: '#2C2416' }}
          role="status"
          aria-live="polite"
        >
          <span className="min-w-0 flex-1 break-words text-sm font-semibold">{rollAlert}</span>
          <button
            type="button"
            onClick={() => setRollAlert(null)}
            className="shrink-0 px-1 text-lg leading-none"
            style={{ color: '#7A6F5C' }}
            aria-label="Dismiss roll alert"
          >
            ×
          </button>
        </div>
      )}

      {/* Main Content */}
      <div className="flex min-h-[calc(100vh-80px)] flex-col md:h-[calc(100vh-80px)] md:flex-row">
        {/* Left Panel - Roll Inputs */}
        <div
          className="order-2 w-full min-w-0 border-b overflow-y-auto p-4 flex flex-col gap-4 sm:p-6 md:order-1 md:w-1/3 md:border-b-0 md:border-r"
          style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC' }}
        >
          {/* Tab Switcher */}
          <div className="flex gap-2 mb-4">
            <button
              onClick={() => setActiveTab('custom')}
              className="min-w-0 flex-1 px-2 py-1.5 text-xs rounded transition-all border sm:px-3 sm:py-2 sm:text-sm"
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
              className="min-w-0 flex-1 px-2 py-1.5 text-xs rounded transition-all border sm:px-3 sm:py-2 sm:text-sm"
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
            <CustomRolls
              onRoll={handleRoll}
              rollingStat={rollingStat}
              onError={setError}
            />
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
    className="min-w-0 flex-1 px-1 py-1.5 text-[11px] font-semibold rounded transition-all hover:opacity-90 disabled:opacity-60 disabled:cursor-not-allowed sm:py-2 sm:text-xs"
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
    className="min-w-0 flex-1 px-1 py-1.5 text-[11px] font-semibold rounded transition-all hover:opacity-90 disabled:opacity-60 disabled:cursor-not-allowed sm:py-2 sm:text-xs"
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
        <RollLog rolls={rolls} onVisibilityChange={handleRollLogVisibilityChange} />
      </div>
    </div>
  )
}
