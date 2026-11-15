import type { ScifiTalent } from '../../types/scifiTalent';
import PanelCard from '../PanelCard';
import { Star } from 'lucide-react';
import Expandable from './Expandable';

/** Displays a list of talents. */
export default function TalentsCard({ talents }: { talents: ScifiTalent[] }) {
  return (
    <PanelCard title="Talents" icon={<Star />} bodyClassName="flex flex-col gap-4">
      <div className="flex flex-col gap-3">
        {talents.map((talent) => (
          <TalentTile key={talent.name} talent={talent} />
        ))}
      </div>
    </PanelCard>
  );
}

export function TalentTile({ talent }: { talent: ScifiTalent }) {
  return <Expandable name={talent.name} description={talent.description} />;
}
