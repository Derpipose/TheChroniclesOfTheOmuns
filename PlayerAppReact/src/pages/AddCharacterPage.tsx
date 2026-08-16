import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useScifiRaces, useScifiSubracesByRace, useScifiClasses, useScifiBackgrounds } from '../hooks/useScifiReference'
import { createCharacter } from '../api/characterApi'
import { createScifiCharacter } from '../api/scifiCharacterApi'
import { bulkUpdateCharacterStats } from '../api/characterStatsApi'
import type { ScifiSubrace } from '../types/scifiReference'

export default function AddCharacterPage() {
  const navigate = useNavigate()

  const [name, setName] = useState('')
  const [level, setLevel] = useState('1')
  const [raceId, setRaceId] = useState<number | undefined>(undefined)
  const [subraceId, setSubraceId] = useState<number | undefined>(undefined)
  const [classId, setClassId] = useState<number | undefined>(undefined)
  const [backgroundId, setBackgroundId] = useState<number | undefined>(undefined)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const { data: races } = useScifiRaces()
  const { data: subraces } = useScifiSubracesByRace(raceId)
  const { data: classes } = useScifiClasses()
  const { data: backgrounds } = useScifiBackgrounds()

  const [strength, setStrength] = useState('0')
  const [constitution, setConstitution] = useState('0')
  const [dexterity, setDexterity] = useState('0')
  const [wisdom, setWisdom] = useState('0')
  const [charisma, setCharisma] = useState('0')
  const [intelligence, setIntelligence] = useState('0')
  const statFields: { label: string; value: string; setValue: (v: string) => void }[] = [
  { label: 'Strength', value: strength, setValue: setStrength },
  { label: 'Constitution', value: constitution, setValue: setConstitution },
  { label: 'Dexterity', value: dexterity, setValue: setDexterity },
  { label: 'Wisdom', value: wisdom, setValue: setWisdom },
  { label: 'Charisma', value: charisma, setValue: setCharisma },
  { label: 'Intelligence', value: intelligence, setValue: setIntelligence },
]

  // ??? — you write this part: find the selected race object from `races`, given `raceId`
  const selectedRace = races?.find((r) => r.id === raceId);
  const selectedSubrace = subraces?.find((sr) => sr.id === subraceId);
  const subraceHasStats = (subrace: ScifiSubrace | undefined): boolean => {
    return subrace !== undefined && (subrace.raceStats.modifiers.length > 0 || subrace.raceStats.freeStatPicks.length > 0);
  }

  const activeRaceStats = subraceHasStats(selectedSubrace) ? selectedSubrace?.raceStats : selectedRace?.raceStats;
  const activeRaceFreeStatPicks = subraceHasStats(selectedSubrace) ? selectedSubrace?.raceStats.freeStatPicks : selectedRace?.raceStats.freeStatPicks;
  // const getRacialModifiers = selectedRace?.raceStats.modifiers || selectedSubrace?.raceStats.modifiers || [];
  // const getRacialFreeStatPicks = selectedRace?.raceStats.freeStatPicks ||  selectedSubrace?.raceStats.freeStatPicks || [];

  const getRacialModifierForStat = (statLabel: string): number => {
    const modifiers = activeRaceStats?.modifiers || [];
    const modifier = modifiers.find((m) => m.stat === statLabel);
    return modifier ? modifier.modifier : 0;
  }

  /* ... */

  // ??? — should the subrace dropdown even render? depends on two things
  const showSubraceDropdown =  subraces && subraces.length > 0 //selectedRace?.hasSubrace &&
  // console.log('selectedRace:', selectedRace)
  // console.log('subraces:', subraces)
  // console.log('showSubraceDropdown:', showSubraceDropdown)
  /* ... */

  // ...

  const handleAdd = async () => {
    if (!name.trim()) {
      setError('Name is required')
      return
    }

    setSubmitting(true)
    setError(null)

    try {
      // step 1: create base character, get back its id
      const characterResponse = await createCharacter({ name, level: parseInt(level) })
      const characterId = characterResponse.id

      // step 2: use that id to create the scifi character, passing raceId/subraceId/classId/backgroundId
      await createScifiCharacter({
        characterId,
        scifiRaceId : raceId,
        scifiSubraceId : subraceId,
        scifiClassId : classId,
        scifiBackgroundId : backgroundId,
      })

      // step 3: update the character's stats
      await bulkUpdateCharacterStats(characterId, {
        strength: parseInt(strength),
        constitution: parseInt(constitution),
        dexterity: parseInt(dexterity),
        wisdom: parseInt(wisdom),
        charisma: parseInt(charisma),
        intelligence: parseInt(intelligence),
      })

      // step 4: navigate to /characters
      navigate('/characters')
    } catch (err) {
      setError('Failed to add character. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
  <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
    <div className="border-b px-6 py-4 flex items-center gap-3" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
      <button onClick={() => navigate('/characters')} className="text-sm transition-colors hover:underline" style={{ color: '#7A6F5C' }}>
        ← Back
      </button>
      <h1 className="text-lg font-semibold" style={{ color: '#2C2416' }}>Add Character</h1>
    </div>

    <div className="max-w-md mx-auto px-6 py-8">
      <div className="rounded-2xl border p-6 flex flex-col gap-5" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>

        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
            placeholder="Character name"
          />
        </div>

        {/* YOUR TURN: Race dropdown — options from `races`, onChange sets raceId, also reset subraceId to undefined when race changes */}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Race</label>
          <select
            value={raceId ?? ''}
            onChange={(e) => {
              const selectedId = e.target.value ? parseInt(e.target.value) : undefined
              setRaceId(selectedId)
              setSubraceId(undefined) // Reset subrace when race changes
            }}
            className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
          >
            <option value="">Select a race</option>
            {races?.map((race) => (
              <option key={race.id} value={race.id}>
                {race.name}
              </option>
            ))}
          </select>
        </div>

        {/* YOUR TURN: Subrace dropdown — only rendered if showSubraceDropdown is true, options from `subraces` */}
        {showSubraceDropdown && (
          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Subrace</label>
            <select
              value={subraceId ?? ''}
              onChange={(e) => setSubraceId(e.target.value ? parseInt(e.target.value) : undefined)}
              className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
              style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
            >
              <option value="">Select a subrace</option>
              {subraces?.map((subrace) => (
                <option key={subrace.id} value={subrace.id}>
                  {subrace.subRaceName}
                </option>
              ))}
            </select>
          </div>
        )}

        {/* YOUR TURN: Class dropdown — options from `classes` */}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Class</label>
          <select
            value={classId ?? ''}
            onChange={(e) => setClassId(e.target.value ? parseInt(e.target.value) : undefined)}
            className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
          >
            <option value="">Select a class</option>
            {classes?.map((cls) => (
              <option key={cls.id} value={cls.id}>
                {cls.name}
              </option>
            ))}
          </select>
        </div>

        {/* YOUR TURN: Background dropdown — options from `backgrounds` */}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Background</label>
          <select
            value={backgroundId ?? ''}
            onChange={(e) => setBackgroundId(e.target.value ? parseInt(e.target.value) : undefined)}
            className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
          >
            <option value="">Select a background</option>
            {backgrounds?.map((bg) => (
              <option key={bg.id} value={bg.id}>
                {bg.name}
              </option>
            ))}
          </select>
        </div>

        {/* Stats inputs — strength, constitution, dexterity, wisdom, charisma, intelligence */}
        <div className="grid grid-cols-2 gap-3">
          {statFields.map(({ label, value, setValue }) => {
            const racialEntry = activeRaceStats?.modifiers.find((m) => m.stat === label); /* ??? — find the matching modifier entry for this stat's label */
            const isLocked =  racialEntry?.isLocked ?? false; /* ??? */
            const racialMod = racialEntry?.modifier ?? 0; /* ??? */
            const finalValue = isLocked ? racialMod : (parseInt(value) || 0) + (racialMod) + 10; /* ??? — branch: locked uses racialMod directly, unlocked uses the formula */

            return (
              <div key={label} className="flex flex-col gap-1">
                <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>{label}</label>

                {isLocked ? (
                  <span className="text-sm" style={{ color: '#2C2416' }}>
                    {/* ??? — render: {racialMod} (locked by race) */}
                    {racialMod} (locked by race)
                  </span>
                ) : (
                  <div className="flex items-center gap-2">
                    <input
                      type="number"
                      min="1"
                      max="10"
                      value={value}
                      onChange={(e) => setValue(e.target.value)}
                      className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors w-16"
                      style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                    />
                    <span className="text-sm" style={{ color: '#7A6F5C' }}>
                      {/* ??? — render: + {racialMod} (racial) + 10 = {finalValue} */}
                      + {racialMod} (racial) + 10 = {finalValue}
                    </span>
                  </div>
                )}
              </div>
            )
          })}
        </div>

        {/* Level input */}

        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Level (Currenly set to only 1)</label>
          <input
            type="number"
            min="1"
            max="20"
            value={level}
            disabled = {true}
            onChange={(e) => setLevel(e.target.value)}
            className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
            style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
          />
        </div>

        {error && <p className="text-sm" style={{ color: '#A85C4B' }}>{error}</p>}

        <div className="flex gap-3 mt-2">
          <button onClick={() => navigate('/characters')} className="flex-1 rounded-lg py-2 text-sm font-medium transition-colors" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
            Cancel
          </button>
          <button onClick={handleAdd} disabled={submitting} className="flex-1 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50" style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}>
            {submitting ? 'Adding…' : 'Add Character'}
          </button>
        </div>
      </div>
    </div>
  </div>
)
}
