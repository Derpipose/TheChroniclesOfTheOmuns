import PanelCard from '../PanelCard'
import { Dices } from 'lucide-react'
import type { StatEntry } from './StatEntry'

/** Mobile ability scores as label/value rows. */
export default function StatListCard({ statEntries }: { statEntries: StatEntry[] }) {
  return (
    <PanelCard title="Stats" icon={<Dices />}>
      <div className="grid grid-cols-2 gap-3">
        {statEntries.map(({ label, value }) => (
          <div key={label} className="row-chip">
            <span className="text-xs font-medium uppercase tracking-wide fg">{label}</span>
            <span className="text-sm font-semibold fg">{value}</span>
          </div>
        ))}
      </div>
    </PanelCard>
  )
}
