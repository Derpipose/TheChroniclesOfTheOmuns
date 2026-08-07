import { useState, useEffect } from 'react'
import { resolveExpression} from '../functions/DiceRoller'

interface DiceRoll {
  id: string
  timestamp: string
  expression: string
  resolvedExpr: string
  allRolls: number[]
  total: number
  label?: string
}

interface CustomRollPreset {
  id: string
  name: string
  expression: string
}

export default function DiceRollerPage() {
  const [activeTab, setActiveTab] = useState<'custom' | 'character'>('custom')
  const [rolls, setRolls] = useState<DiceRoll[]>([])
  const [customPresets, setCustomPresets] = useState<CustomRollPreset[]>([])
  const [newPresetName, setNewPresetName] = useState('')
  const [newPresetExpr, setNewPresetExpr] = useState('')
  const [showAddPreset, setShowAddPreset] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // Load custom presets from localStorage on mount
  useEffect(() => {
    const saved = localStorage.getItem('diceroller_presets')
    if (saved) {
      try {
        setCustomPresets(JSON.parse(saved))
      } catch (e) {
        console.error('Failed to load presets:', e)
      }
    }
  }, [])

  // Save custom presets to localStorage whenever they change
  useEffect(() => {
    localStorage.setItem('diceroller_presets', JSON.stringify(customPresets))
  }, [customPresets])

  const handleRoll = (expression: string, label?: string) => {
    try {
      setError(null)
      const result = resolveExpression(expression)
      const roll: DiceRoll = {
        id: Date.now().toString(),
        timestamp: new Date().toLocaleTimeString(),
        expression,
        resolvedExpr: result.resolvedExpr,
        allRolls: result.allRolls,
        total: result.total,
        label,
      }
      setRolls((prev) => [...prev, roll]) // append to bottom
    } catch (err) {
      setError((err as Error).message || 'Invalid dice expression')
    }
  }

  const clearRolls = () => {
    setRolls([])
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
      setCustomPresets((prev) => [...prev, preset])
      setNewPresetName('')
      setNewPresetExpr('')
      setShowAddPreset(false)
      setError(null)
    } catch (err) {
      setError((err as Error).message || 'Invalid dice expression')
    }
  }

  const deletePreset = (id: string) => {
    setCustomPresets((prev) => prev.filter((p) => p.id !== id))
  }

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
      {/* Header */}
      <div
        className="border-b px-6 py-4 flex items-center justify-between"
        style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
      >
        <h1 className="text-lg font-semibold" style={{ color: '#2C2416' }}>
          Dice Roller
        </h1>
        <button
          onClick={clearRolls}
          className="px-4 py-2 text-sm rounded transition-colors hover:opacity-90 disabled:opacity-50"
          style={{
            backgroundColor: '#8B7355',
            color: '#FDFAF4',
          }}
          disabled={rolls.length === 0}
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

              {customPresets.map((preset) => (
                <div
                  key={preset.id}
                  className="rounded-lg border p-3 space-y-2"
                  style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
                >
                  <div className="flex items-center justify-between gap-2">
                    <div className="flex-1 min-w-0">
                      <p
                        className="text-sm font-semibold truncate"
                        style={{ color: '#2C2416' }}
                      >
                        {preset.name}
                      </p>
                      <p
                        className="text-xs mt-1 break-all font-mono"
                        style={{ color: '#7A6F5C' }}
                      >
                        {preset.expression}
                      </p>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleRoll(preset.expression, preset.name)}
                      className="flex-1 py-2 text-xs font-semibold rounded transition-colors hover:opacity-90"
                      style={{
                        backgroundColor: '#8B7355',
                        color: '#FDFAF4',
                      }}
                    >
                      Roll
                    </button>
                    <button
                      onClick={() => deletePreset(preset.id)}
                      className="py-2 px-3 text-xs rounded transition-colors hover:opacity-90"
                      style={{
                        backgroundColor: '#EDE8DC',
                        color: '#5C4F38',
                      }}
                    >
                      ×
                    </button>
                  </div>
                </div>
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
              <div
                className="rounded-lg border p-4 text-center"
                style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
              >
                <p
                  className="text-sm font-medium"
                  style={{ color: '#2C2416' }}
                >
                  TODO: Wire up to character stats & weapons
                </p>
                <p
                  className="text-xs mt-2"
                  style={{ color: '#7A6F5C' }}
                >
                  This will pull ability checks, skill rolls, and weapon attacks
                  from the selected character's data.
                </p>
              </div>
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
                      <p
                        className="text-2xl font-bold font-mono"
                        style={{ color: '#8B7355' }}
                      >
                        {roll.total}
                      </p>
                    </div>
                  </div>

                  <div className="space-y-1 pt-2 border-t" style={{ borderColor: '#DDD8CC' }}>
                    <p
                      className="text-xs font-mono"
                      style={{ color: '#2C2416' }}
                    >
                      {roll.expression}
                    </p>
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
