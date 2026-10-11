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
      <div className="label">
        Custom Presets
      </div>

      {presets.length === 0 && !showAddPreset && (
        <p className="text-xs text-center fg-muted">
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
          className="rounded-lg border p-3 space-y-3 panel"
        >
          <input
            type="text"
            placeholder="Preset name"
            value={newPresetName}
            onChange={(event) => setNewPresetName(event.target.value)}
            className="field-input w-full"
          />
          <input
            type="text"
            placeholder="e.g., 1d20 + 3"
            value={newPresetExpr}
            onChange={(event) => setNewPresetExpr(event.target.value)}
            className="field-input w-full font-mono"
          />
          <div className="flex gap-2">
            <button
              onClick={addPreset}
              className="btn-sm btn-accent min-w-0 flex-1 font-semibold"
            >
              Add
            </button>
            <button
              onClick={cancelAddPreset}
              className="btn-sm btn-neutral min-w-0 flex-1 font-medium"
            >
              Cancel
            </button>
          </div>
        </div>
      ) : (
        <button
          onClick={() => setShowAddPreset(true)}
          className="btn-sm btn-neutral w-full border border-subtle py-2 font-medium"
        >
          + New Preset
        </button>
      )}
    </div>
  )
}
