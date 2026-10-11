import Card from '../Card'
import type { StatEntry } from './StatEntry'
import StatTile from './StatTile'

/** Desktop grid of three-letter ability score tiles. */
export default function AbilityScoresCard({ statEntries }: { statEntries: StatEntry[] }) {
  return (
    <Card title="Ability Scores">
      <div className="grid grid-cols-3 gap-3 mt-3">
        {statEntries.map(({ label, value }) => (
          <StatTile key={label} label={label.slice(0, 3)} value={value} labelSize="text-[10px]" />
        ))}
      </div>
    </Card>
  )
}
