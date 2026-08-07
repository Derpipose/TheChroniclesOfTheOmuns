export interface CustomRollPreset {
  id: string
  name: string
  expression: string
}

interface CustomRollPresetCardProps {
  preset: CustomRollPreset
  onRoll: (expression: string, label?: string) => void
  onDelete: (id: string) => void
}

export function CustomRollPresetCard({
  preset,
  onRoll,
  onDelete,
}: CustomRollPresetCardProps) {
  return (
    <div
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
          onClick={() => onRoll(preset.expression, preset.name)}
          className="flex-1 py-2 text-xs font-semibold rounded transition-colors hover:opacity-90"
          style={{
            backgroundColor: '#8B7355',
            color: '#FDFAF4',
          }}
        >
          Roll
        </button>
        <button
          onClick={() => onDelete(preset.id)}
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
  )
}
