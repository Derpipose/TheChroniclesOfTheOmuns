import { updateCharacterNotes } from "../api/characterApi"

export interface CharacterNotesProps {
  characterId: number | undefined
  notes: string
  onNotesChange: (newNotes: string) => void
}

export function CharacterNotes({ characterId, notes, onNotesChange }: CharacterNotesProps) {
  const handleNotesChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    onNotesChange(event.target.value)
  }

  const handleSaveNotes = async (_event: React.FocusEvent<HTMLTextAreaElement>) => {
    if (characterId !== undefined) {
      try {
        await updateCharacterNotes(characterId, notes)
      } catch (error) {
        console.error('Failed to update character notes:', error)
      }
    }
  }

  return (
    <div className="flex flex-col gap-1">
      <label
        htmlFor={`character-notes-${characterId ?? 'new'}`}
        className="text-xs font-medium uppercase tracking-wide"
        style={{ color: 'var(--color-text-60)' }}
      >
        Notes
      </label>
      <textarea
        id={`character-notes-${characterId ?? 'new'}`}
        value={notes}
        onChange={handleNotesChange}
        onBlur={handleSaveNotes}
        rows={4}
        className="field-input"
        style={{ backgroundColor: 'var(--color-background)', borderColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}
      />
    </div>
  )
}