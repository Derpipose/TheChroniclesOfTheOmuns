import type { ScifiBackground, ScifiClass, ScifiRace, ScifiSubrace } from '../../types/scifiReference'
import Card from '../Card'
import Expandable from './Expandable'

export function RacialTraitsCard({ race, subrace }: { race: ScifiRace; subrace?: ScifiSubrace }) {
  return (
    <Card title="Racial Traits" className="flex flex-col gap-3">
      <Expandable name={race.name} description={race.description} />
      {subrace && (
        <div className="pt-3 border-t border-subtle">
          <Expandable name={subrace.subRaceName} description={subrace.description} />
        </div>
      )}
    </Card>
  )
}

export function ClassCard({ scifiClass }: { scifiClass: ScifiClass }) {
  return (
    <Card title="Class" className="flex flex-col gap-3">
      <div>
        <p className="text-sm font-semibold fg">{scifiClass.name}</p>
        {scifiClass.description && <p className="text-sm mt-1 fg-muted">{scifiClass.description}</p>}
      </div>
    </Card>
  )
}

export function BackgroundCard({ background }: { background: ScifiBackground }) {
  return (
    <Card title="Background" className="flex flex-col gap-3">
      <Expandable name={background.name} description={background.description} />
    </Card>
  )
}
