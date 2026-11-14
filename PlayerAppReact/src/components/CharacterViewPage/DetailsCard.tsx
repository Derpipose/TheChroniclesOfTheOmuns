import PanelCard from '../PanelCard'
import { Info } from 'lucide-react'
import DetailRow from './DetailRow'

interface DetailsCardProps {
  race?: string
  subrace?: string
  showSubrace: boolean
  characterClass?: string
  background?: string
}

/** Mobile list of race, subrace, class and background names. */
export default function DetailsCard({ race, subrace, showSubrace, characterClass, background }: DetailsCardProps) {
  return (
    <PanelCard title="Details" icon={<Info />}>
      <div className="flex flex-col gap-2 text-sm fg">
        <DetailRow label="Race" value={race} />
        {showSubrace && <DetailRow label="Subrace" value={subrace} />}
        <DetailRow label="Class" value={characterClass} />
        <DetailRow label="Background" value={background} />
      </div>
    </PanelCard>
  )
}
