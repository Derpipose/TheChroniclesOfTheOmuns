import type { AdjustedStats } from '../../types/scifiCharacterWithStats'
import Card from '../Card'
import StatTile from './StatTile'

/** AC, DR and HP tiles with a health bar. */
export default function VitalsCard({ adjustedStats }: { adjustedStats: AdjustedStats }) {
  const { currentHealth, maxHealth } = adjustedStats
  const healthPercent = currentHealth > 0 ? Math.min(100, (currentHealth / maxHealth) * 100) : 0

  return (
    <Card className="flex flex-col gap-4">
      <div className="grid grid-cols-2 gap-3">
        <div className="flex flex-col gap-3">
          <StatTile label="AC" value={adjustedStats.armorClass} py="py-2" valueSize="text-xl" />
          <StatTile label="DR" value={adjustedStats.damageResist} py="py-2" valueSize="text-xl" />
        </div>
        <StatTile label="HP" value={`${currentHealth} / ${maxHealth}`} py="py-4" valueSize="text-2xl" />
      </div>
      <div className="progress">
        <div className="progress-fill" style={{ width: `${healthPercent}%` }} />
      </div>
    </Card>
  )
}
