import { useLayoutEffect, useRef } from "react"
import { updateCharacterNotes } from "../api/characterApi"

const MIN_ROWS = 4
const MAX_GROWTH = 5 // the box may grow to 5x its starting height, then scrolls

export interface CharacterNotesProps {
  characterId: number | undefined
  notes: string
  onNotesChange: (newNotes: string) => void
}

export function CharacterNotes({ characterId, notes, onNotesChange }: CharacterNotesProps) {
  const textareaRef = useRef<HTMLTextAreaElement>(null)

  // Grow with the content: start at MIN_ROWS tall, cap at MAX_GROWTH x that height.
  useLayoutEffect(() => {
    const el = textareaRef.current
    if (!el) return
    const style = getComputedStyle(el)
    const lineHeight = parseFloat(style.lineHeight) || 20
    const chrome = ['paddingTop', 'paddingBottom', 'borderTopWidth', 'borderBottomWidth']
      .reduce((sum, key) => sum + (parseFloat(style[key as 'paddingTop']) || 0), 0)
    const base = lineHeight * MIN_ROWS + chrome
    el.style.height = 'auto'
    const wanted = el.scrollHeight + (parseFloat(style.borderTopWidth) || 0) + (parseFloat(style.borderBottomWidth) || 0)
    el.style.height = `${Math.min(Math.max(wanted, base), base * MAX_GROWTH)}px`
  }, [notes])

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
    <textarea
      ref={textareaRef}
      aria-label="Character notes"
      value={notes}
      onChange={handleNotesChange}
      onBlur={handleSaveNotes}
      rows={MIN_ROWS}
      className="field-input resize-none"
    />
  )
}
