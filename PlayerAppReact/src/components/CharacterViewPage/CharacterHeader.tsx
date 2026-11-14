import type { Character } from '../../types/character'
import type { ScifiBackground } from '../../types/scifiReference'
import Card from '../Card'
import StatusPills from './StatusPills'

interface CharacterHeaderProps {
  character: Character
  raceName?: string
  characterClass?: string
  background?: ScifiBackground
  setting: string
  onEdit: () => void
}

/** Desktop header card: initial, name, summary line, status and edit action. */
export default function CharacterHeader({ character, raceName, characterClass, background, setting, onEdit }: CharacterHeaderProps) {
  return (
    <Card className="flex items-center justify-between gap-6">
      <div className="flex items-center gap-5">
        <div className="avatar">{character.name.charAt(0).toUpperCase()}</div>
        <div className="flex flex-col gap-1">
          <h2 className="text-3xl font-bold fg">{character.name}</h2>
          <p className="text-sm fg-muted">
            {[raceName, characterClass, `Level ${character.level}`].filter(Boolean).join(' • ')}
          </p>
          {background && (
            <p className="text-sm fg-muted">
              Background: <span className="fg">{background.name}</span>
            </p>
          )}
        </div>
      </div>
      <div className="flex items-center gap-3 shrink-0">
        <StatusPills character={character} setting={setting} />
        <button onClick={onEdit} className="btn btn-accent px-5">Edit Character</button>
      </div>
    </Card>
  )
}
