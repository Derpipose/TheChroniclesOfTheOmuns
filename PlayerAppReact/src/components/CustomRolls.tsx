import { useEffect, useState } from 'react'
import { resolveExpression } from '../functions/DiceRoller'
import { CustomRollPresetCard, type CustomRollPreset } from './CustomRollPresetCard'

interface CustomRollsProps {
  onRoll: (expression: string, label?: string, rollKey?: string) => void
  rollingStat: string | null
  onError: (message: string | null) => void
}

export default function CustomRolls({
  onRoll,
  rollingStat,
  onError,
}: CustomRollsProps) {
  const [presets, setPresets] = useState<CustomRollPreset[]>(() => {
    const saved = localStorage.getItem('diceroller_presets')
    if (!saved) return []

    try {
      return JSON.parse(saved) as CustomRollPreset[]
    } catch (error) {
      console.error('Failed to load presets:', error)
      return []
    }
  })
  const [newPresetName, setNewPresetName] = useState('')
  const [newPresetExpr, setNewPresetExpr] = useState('')
  const [showAddPreset, setShowAddPreset] = useState(false)

  useEffect(() => {
    localStorage.setItem('diceroller_presets', JSON.stringify(presets))
  }, [presets])

  const addPreset = () => {
    if (!newPresetName.trim() || !newPresetExpr.trim()) {
      onError('Name and expression are required')
      return
    }

    try {
      resolveExpression(newPresetExpr)
      setPresets((previousPresets) => [
        ...previousPresets,
        {
          id: Date.now().toString(),
          name: newPresetName,
          expression: newPresetExpr,
        },
      ])
      setNewPresetName('')
      setNewPresetExpr('')
      setShowAddPreset(false)
      onError(null)
    } catch (error) {
      onError((error as Error).message || 'Invalid dice expression')
    }
  }

  const deletePreset = (id: string) => {
    setPresets((previousPresets) => previousPresets.filter((preset) => preset.id !== id))
  }

  const cancelAddPreset = () => {
    setShowAddPreset(false)
    setNewPresetName('')
    setNewPresetExpr('')
  }

  return (
    <div className="space-y-4">
      <div className="text-xs font-medium uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>
        Custom Presets
      </div>

      {presets.length === 0 && !showAddPreset && (
        <p className="text-xs text-center" style={{ color: 'var(--color-text-60)' }}>
          No presets yet
        </p>
      )}

      {presets.map((preset) => (
        <CustomRollPresetCard
          key={preset.id}
          preset={preset}
          onRoll={onRoll}
          onDelete={deletePreset}
          isRolling={rollingStat === `preset-${preset.id}`}
        />
      ))}

      {showAddPreset ? (
        <div
          className="rounded-lg border p-3 space-y-3"
          style={{ backgroundColor: 'var(--color-background-30)', borderColor: 'var(--color-primary-30)' }}
        >
          <input
            type="text"
            placeholder="Preset name"
            value={newPresetName}
            onChange={(event) => setNewPresetName(event.target.value)}
            className="w-full px-3 py-2 text-sm rounded outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: 'var(--color-background)', borderColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}
          />
          <input
            type="text"
            placeholder="e.g., 1d20 + 3"
            value={newPresetExpr}
            onChange={(event) => setNewPresetExpr(event.target.value)}
            className="w-full px-3 py-2 text-sm rounded outline-none border focus:border-stone-400 transition-colors font-mono"
            style={{ backgroundColor: 'var(--color-background)', borderColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}
          />
          <div className="flex gap-2">
            <button
              onClick={addPreset}
              className="min-w-0 flex-1 px-1 py-1.5 text-[11px] font-semibold rounded transition-colors hover:opacity-90 sm:py-2 sm:text-xs"
              style={{ backgroundColor: 'var(--color-accent)', color: 'var(--color-text)' }}
            >
              Add
            </button>
            <button
              onClick={cancelAddPreset}
              className="min-w-0 flex-1 px-1 py-1.5 text-[11px] font-medium rounded transition-colors sm:py-2 sm:text-xs"
              style={{ backgroundColor: 'var(--color-background-60)', color: 'var(--color-text)' }}
            >
              Cancel
            </button>
          </div>
        </div>
      ) : (
        <button
          onClick={() => setShowAddPreset(true)}
          className="w-full py-2 text-xs font-medium rounded transition-colors border"
          style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}
        >
          + New Preset
        </button>
      )}
    </div>
  )
}
