export interface CustomRollPreset {
  id: string
  name: string
  expression: string
}

interface CustomRollPresetCardProps {
  preset: CustomRollPreset
  onRoll: (expression: string, label?: string, rollKey?: string) => void
  onDelete: (id: string) => void
  isRolling?: boolean
}

export function CustomRollPresetCard({
  preset,
  onRoll,
  onDelete,
  isRolling = false,
}: CustomRollPresetCardProps) {
  return (
    <div
      className="rounded-lg border p-3 space-y-2 panel"
    >
      <div className="flex items-center justify-between gap-2">
        <div className="flex-1 min-w-0">
          <p
            className="text-sm font-semibold truncate fg"
          >
            {preset.name}
          </p>
          <p
            className="text-xs mt-1 break-all font-mono fg-muted"
          >
            {preset.expression}
          </p>
        </div>
      </div>
      <div className="flex gap-2">
        <button
          onClick={() => onRoll(preset.expression, preset.name, `preset-${preset.id}`)}
          disabled={isRolling}
          className="btn-sm btn-accent min-w-0 flex-1 font-semibold"
        >
          {isRolling ? 'Rolling' : 'Roll'}
        </button>
        <button
          onClick={() => onDelete(preset.id)}
          className="btn-sm btn-neutral shrink-0"
        >
          ×
        </button>
      </div>
    </div>
  )
}
