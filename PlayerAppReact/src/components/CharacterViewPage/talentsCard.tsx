import type { ScifiTalent } from '../../types/scifiTalent';
import PanelCard from '../PanelCard';
import { Star } from 'lucide-react';

/** Displays a list of talents. */
export default function TalentsCard({ talents }: { talents: ScifiTalent[] }) {
  return (
    <PanelCard title="Talents" icon={<Star />} bodyClassName="flex flex-col gap-4">
      <div className="grid grid-cols-2 gap-3">
        {talents.map((talent) => (
          <TalentTile key={talent.name} talent={talent} />
        ))}
      </div>
    </PanelCard>
  );
}

export function TalentTile({ talent }: { talent: ScifiTalent }) {
  return (
    <div className="talent-tile">
      <span className="talent-tile-name">{talent.name}</span>
        <details className="talent-tile-description">
          <summary className="talent-tile-summary">Description</summary>
          <p>{talent.description}</p>
        </details>
    </div>
  );
}