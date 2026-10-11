import type { Character } from '../../types/character'
import type { AdjustedStats } from '../../types/scifiCharacterWithStats'
import Card from '../Card'
import Pill from '../Pill'
import StatusPills from './StatusPills'

interface OverviewCardProps {
  character: Character
  adjustedStats?: AdjustedStats
  setting: string
}

/** Mobile summary: name, status, level, HP, AC and DR. */
export default function OverviewCard({ character, adjustedStats, setting }: OverviewCardProps) {
  return (
    <Card className="flex flex-col gap-3">
      <span className="card-title font-medium">{character.name}</span>
      <div>
        <StatusPills character={character} setting={setting} />
      </div>
      <div className="flex flex-wrap items-center gap-3">
        <Pill>Level {character.level}</Pill>
        <Pill>{character.currentHealth} HP</Pill>
        <Pill>AC {adjustedStats?.armorClass ?? '—'}</Pill>
        <Pill>DR {adjustedStats?.damageResist ?? '—'}</Pill>
      </div>
    </Card>
  )
}
