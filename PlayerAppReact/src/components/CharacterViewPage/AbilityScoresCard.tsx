import PanelCard from '../PanelCard'
import { Dices } from 'lucide-react'
import type { StatEntry } from './StatEntry'
import StatTile from './StatTile'

/** Desktop grid of three-letter ability score tiles. */
export default function AbilityScoresCard({ statEntries }: { statEntries: StatEntry[] }) {
  return (
    <PanelCard title="Ability Scores" icon={<Dices />}>
      <div className="grid grid-cols-3 gap-3">
        {statEntries.map(({ label, value }) => (
          <StatTile key={label} label={label.slice(0, 3)} value={value} labelSize="text-[10px]" />
        ))}
      </div>
    </PanelCard>
  )
}
