import { useState, useEffect,  } from 'react'
import { resolveExpression} from '../functions/DiceRoller'
import CustomRolls from '../components/CustomRolls'
import RollLog, { type DiceRoll, type RollMode } from '../components/RollLog'
import NavBar from '../components/NavBar'
import { useCharacter, useCharacters, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useCharacterStats } from '../hooks/useCharacterStats'
import { useScifiStatsByCharacterId } from '../hooks/useScifiStats'
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


  const { data: selectedScifiStats, isLoading: adjustedLoading } = useScifiStatsByCharacterId(selectedCharacter?.id)

  console.debug('selectedScifiStats', selectedScifiStats)

  const effectiveStats = selectedScifiStats ?? selectedStats

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
    <div className="dice-page">
      <NavBar title="Dice Roller" />
      {/* Header */}
      <div className="dice-toolbar">
        <div className="flex flex-wrap items-center gap-2">
          <span className="label">
            Roll mode
          </span>
          {(['standard', 'advantage', 'disadvantage'] as RollMode[]).map((mode) => (
            <label
              key={mode}
                className={`mode-chip ${rollMode === mode ? 'mode-chip-active' : ''}`}
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
          className="btn-sm btn-accent w-full sm:w-auto"
          disabled={rolls.length === 0 || rollingStat !== null}
        >
          Clear Log
        </button>
      </div>

      {rollAlert && (
        <div
          className="toast inset-x-4 top-24 sm:inset-x-auto sm:right-6 sm:w-auto sm:min-w-72"
          role="status"
          aria-live="polite"
        >
          <span className="min-w-0 flex-1 break-words text-sm font-semibold">{rollAlert}</span>
          <button
            type="button"
            onClick={() => setRollAlert(null)}
            className="shrink-0 px-1 text-lg leading-none fg-muted"
            aria-label="Dismiss roll alert"
          >
            ×
          </button>
        </div>
      )}

      {/* Main Content */}
      <div className="dice-main">
        {/* Left Panel - Roll Inputs */}
        <div className="dice-inputs">
          {/* Tab Switcher */}
          <div className="flex gap-2 mb-4">
            <button
              onClick={() => setActiveTab('custom')}
              className={`tab-btn ${activeTab === 'custom' ? 'tab-btn-active' : ''}`}
            >
              Custom
            </button>
            <button
              onClick={() => setActiveTab('character')}
              className={`tab-btn ${activeTab === 'character' ? 'tab-btn-active' : ''}`}
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
              <div className="label">
                Character Rolls
              </div>

              {charactersLoading ? (
                <p className="text-sm fg-muted">
                  Loading characters...
                </p>
              ) : activeCharacters.length === 0 ? (
                <div
                  className="rounded-lg border p-4 text-center panel"
                >
                  <p className="text-sm font-medium fg">
                    No active characters available.
                  </p>
                </div>
              ) : (
                <>
                  <label className="flex flex-col gap-2">
                    <span className="label">
                      Select Character
                    </span>
                    <select
                      value={selectedCharacterId}
                      onChange={(event) => setSelectedCharacterId(Number(event.target.value))}
                      className="field-input panel w-full"
                    >
                      {activeCharacters.map((character) => (
                        <option key={character.id} value={character.id}>
                          {character.name}
                        </option>
                      ))}
                    </select>
                  </label>

                  {selectedCharacter && (
                    <p className="text-xs fg-muted">
                      Selected: <span className="font-semibold fg">{selectedCharacter.name}</span>
                    </p>
                  )}

                  {statsLoading || adjustedLoading ? (
                    <p className="text-sm fg-muted">
                      Loading character stats...
                    </p>
                  ) : effectiveStats ? (
                    <div className="space-y-4">
                      <h2 className="card-title">
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
                                  className="rounded-lg border p-3 space-y-2 panel"
                                >
                                  <div className="flex items-center justify-between gap-2">
                                    <div className="flex-1 min-w-0">
                                      <p className="text-sm font-semibold fg">
                                        {label}
                                      </p>
                                      <p className="text-xs mt-1 fg-muted">
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
                                    className="btn-sm w-full font-semibold btn-accent"
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
    className="card-title"
  >
    Weapons
  </h2>

  {weaponsLoading ? (
    <p className="text-sm mt-2 fg-muted">
      Loading weapons...
    </p>
  ) : selectedWeapons.length === 0 ? (
    <div
      className="rounded-lg border p-4 mt-2 text-center panel"
    >
      <p className="text-sm font-medium fg">
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
            className={`rounded-lg border p-3 space-y-2 ${isOutOfAmmo ? 'panel-danger' : 'panel'}`}
          >
            <div className="flex items-center justify-between gap-2">
              <div className="flex-1 min-w-0">
                <p
                  className="text-sm font-semibold fg"
                >
                  {weapon.nickname || weapon.name}
                </p>

                <p
                  className="text-xs mt-1 font-mono fg-muted"
                >
                  {damageFormula} {modifierExpression}
                </p>

                <p
                  className="text-xs mt-1 fg-muted"
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
                  className="text-xs fg-accent"
                  title="Favorite"
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
    className="btn-sm btn-accent min-w-0 flex-1 font-semibold"
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
    className={`btn-sm min-w-0 flex-1 border font-semibold ${!isFiring && isOutOfAmmo ? 'btn-alert' : 'btn-neutral border-subtle'}`}
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
                      className="rounded-lg border p-4 text-center panel"
                    >
                      <p className="text-sm font-medium fg">
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
              className="alert-error"
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
