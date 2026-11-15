import type { ScifiBackground, ScifiClass, ScifiRace, ScifiSubrace } from '../../types/scifiReference'
import PanelCard from '../PanelCard'
import { BookOpen, Dna, Swords } from 'lucide-react'
import Expandable from './Expandable'

export function RacialTraitsCard({ race, subrace }: { race: ScifiRace; subrace?: ScifiSubrace }) {
  return (
    <PanelCard title="Racial Traits" icon={<Dna />} bodyClassName="flex flex-col gap-3">
      <Expandable name={race.name} description={race.description} />
      {subrace && (
        <div className="pt-3 border-t border-subtle">
          <Expandable name={subrace.subRaceName} description={subrace.description} />
        </div>
      )}
    </PanelCard>
  )
}

export function ClassCard({ scifiClass }: { scifiClass: ScifiClass }) {
  return (
    <PanelCard title="Class" icon={<Swords />}>
      <div>
        <p className="text-sm font-semibold fg">{scifiClass.name}</p>
        {scifiClass.description && <p className="text-sm mt-1 fg-muted">{scifiClass.description}</p>}
      </div>
    </PanelCard>
  )
}

export function BackgroundCard({ background }: { background: ScifiBackground }) {
  return (
    <PanelCard title="Background" icon={<BookOpen />}>
      <Expandable name={background.name} description={background.description} />
    </PanelCard>
  )
}
