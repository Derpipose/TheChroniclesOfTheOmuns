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
      <div className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
        Custom Presets
      </div>

      {presets.length === 0 && !showAddPreset && (
        <p className="text-xs text-center" style={{ color: '#7A6F5C' }}>
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
          style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
        >
          <input
            type="text"
            placeholder="Preset name"
            value={newPresetName}
            onChange={(event) => setNewPresetName(event.target.value)}
            className="w-full px-3 py-2 text-sm rounded outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
          />
          <input
            type="text"
            placeholder="e.g., 1d20 + 3"
            value={newPresetExpr}
            onChange={(event) => setNewPresetExpr(event.target.value)}
            className="w-full px-3 py-2 text-sm rounded outline-none border focus:border-stone-400 transition-colors font-mono"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
          />
          <div className="flex gap-2">
            <button
              onClick={addPreset}
              className="flex-1 py-2 text-xs font-semibold rounded transition-colors hover:opacity-90"
              style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
            >
              Add
            </button>
            <button
              onClick={cancelAddPreset}
              className="flex-1 py-2 text-xs font-medium rounded transition-colors"
              style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}
            >
              Cancel
            </button>
          </div>
        </div>
      ) : (
        <button
          onClick={() => setShowAddPreset(true)}
          className="w-full py-2 text-xs font-medium rounded transition-colors border"
          style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#5C4F38' }}
        >
          + New Preset
        </button>
      )}
    </div>
  )
}
