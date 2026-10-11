import type { Character } from '../../types/character'
import Pill from '../Pill'

/** Setting and active/edit-mode pills for a character. */
export default function StatusPills({ character, setting }: { character: Character; setting: string }) {
  const variant = character.isActive ? 'active' : 'neutral'

  return (
    <>
      <Pill variant={variant}>{setting}</Pill>
      <Pill variant={variant}>{character.isActive ? 'Active' : 'In Edit Mode'}</Pill>
    </>
  )
}
