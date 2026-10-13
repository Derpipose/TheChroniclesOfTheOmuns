import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useScifiRaces, useScifiSubracesByRace, useScifiClasses, useScifiBackgrounds } from '../hooks/useScifiReference'
import { createCharacter, updateCharacter } from '../api/characterApi'
import { createScifiCharacter, updateScifiCharacter } from '../api/scifiCharacterApi'
import { bulkUpdateCharacterStats } from '../api/characterStatsApi'
import type { ScifiSubrace } from '../types/scifiReference'
import type {StatPick} from '../types/character'
import { useCharacter, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useCharacterStats } from '../hooks/useCharacterStats'
import { CharacterNotes } from '../components/CharacterNotes'

export default function CharacterFormPage({ mode }: { mode: 'add' | 'edit' }) {
    const navigate = useNavigate()

    const { id } = useParams()
    const characterId = id ? parseInt(id, 10) : undefined

    const { data: character } = useCharacter(characterId ?? 0)
    const { data: scifiCharacter } = useScifiCharacterByCharacterId(characterId ?? 0)
    const { data: rawStats } = useCharacterStats(characterId ?? 0)

    const [name, setName] = useState('')
    const [level, setLevel] = useState('1')
    const [notes, setNotes] = useState('')
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

    const selectedRace = races?.find((r) => r.id === raceId);
    const selectedSubrace = subraces?.find((sr) => sr.id === subraceId);
    const subraceHasStats = (subrace: ScifiSubrace | undefined): boolean => {
        return subrace !== undefined && (subrace.raceStats.modifiers.length > 0 || subrace.raceStats.freeStatPicks.length > 0);
    }

    const activeRaceStats = subraceHasStats(selectedSubrace) ? selectedSubrace?.raceStats : selectedRace?.raceStats;

    const [pickAssignments, setPickAssignments] = useState<(string | null)[]>([]);

    // Looks up the racial bonus for a given stat, checking BOTH the race/subrace's
    // fixed modifiers AND whatever the player has assigned via picks, so the live
    // breakdown display stays in sync with pick selections as they're made.
    const getRacialModifierForStat = (statLabel: string): number => {
        const fixedModifiers = activeRaceStats?.modifiers || [];
        const fixedMatch = fixedModifiers.find((m) => m.stat === statLabel);
        if (fixedMatch) return fixedMatch.modifier;

        const pickIndex = pickAssignments.findIndex((assigned) => assigned === statLabel);
        if (pickIndex !== -1) {
            return activeRaceStats?.freeStatPicks[pickIndex] ?? 0;
        }

        return 0;
    }

    const showSubraceDropdown =  subraces && subraces.length > 0

    const getEligibleStatsForPick = (pickIndex: number): string[] => {
        // start with all six stat labels
        let eligibleStats = ['Strength', 'Constitution', 'Dexterity', 'Wisdom', 'Charisma', 'Intelligence'];
        // remove any stat that appears in activeRaceStats?.modifiers (fixed/locked)
        if (activeRaceStats?.modifiers) {
        activeRaceStats?.modifiers.forEach((m) => {
            const index = eligibleStats.indexOf(m.stat);
            if (index !== -1) {
            eligibleStats.splice(index, 1);
            }
        });
        }

        // remove any stat already assigned to a DIFFERENT pick index in pickAssignments
        for (let i = 0; i < pickAssignments.length; i++) {
        if (i === pickIndex) continue;
            const assignedStat = pickAssignments[i];
            if (assignedStat) {
                const index = eligibleStats.indexOf(assignedStat);
                if (index !== -1) eligibleStats.splice(index, 1);
            }
        }
        return eligibleStats;
    }

    const statPicks: StatPick[] = (activeRaceStats?.freeStatPicks ?? [])
        .map((amount, index) => ({ stat: pickAssignments[index], amount }))
        .filter((pick): pick is StatPick => pick.stat !== null)

    const handleSubmit = async () => {

        if (!name.trim()) {
            setError('Name is required')
            return
        }

        setSubmitting(true)
        setError(null)

        try {
            let resolvedCharacterId: number

            if (mode === 'add') {
            // create character, get id
                const characterResponse = await createCharacter({ name, level: parseInt(level), notes })
                const addCharacterId = characterResponse.id
            // create scifi character
                await createScifiCharacter({
                    characterId: addCharacterId,
                    scifiRaceId : raceId,
                    scifiSubraceId : subraceId,
                    scifiClassId : classId,
                    scifiBackgroundId : backgroundId,
                    statPicks,
                })
                resolvedCharacterId = addCharacterId
            } else {
            // guards: characterId undefined? scifiCharacter not loaded?
                if (characterId === undefined) {
                    setError('Missing character id')
                    return
                }

                if (!scifiCharacter) {
                    setError('Scifi character data is not loaded')
                    return
                }
            // update character
                await updateCharacter(characterId, {
                    name: name,
                    level: parseInt(level),
                })
            // update scifi character
                await updateScifiCharacter(scifiCharacter.id, {
                    scifiRaceId: selectedRace?.id,
                    scifiSubraceId: selectedSubrace?.id,
                    scifiClassId: classId,
                    scifiBackgroundId: backgroundId,
                    statPicks,
                })
            resolvedCharacterId = characterId! // or however you settle this
            }

            await bulkUpdateCharacterStats(resolvedCharacterId, {
                strength: parseInt(strength) + 10,
                constitution: parseInt(constitution) + 10,
                dexterity: parseInt(dexterity) + 10,
                wisdom: parseInt(wisdom) + 10,
                charisma: parseInt(charisma) + 10,
                intelligence: parseInt(intelligence) + 10,
                })

            navigate(mode === 'add' ? '/characters' : `/character/${resolvedCharacterId}`)
        } catch (err) {
            setError(mode === 'add' ? 'Failed to add character. Please try again.' : 'Failed to save character. Please try again.')
        } finally {
            setSubmitting(false)
        }

    }

    useEffect(() => {
        if (!character) return;
        setNotes(character.notes);
    }, [character]);

    useEffect(() => {
        if (!character) return;
        setName(character.name);
        setLevel(String(character.level));
    }, [character]);

    useEffect(() => {
        if (!scifiCharacter) return;
        setRaceId(scifiCharacter.scifiRaceId);
        setSubraceId(scifiCharacter.scifiSubraceId);
        setClassId(scifiCharacter.scifiClassId);
        setBackgroundId(scifiCharacter.scifiBackgroundId);
    }, [scifiCharacter]);

    useEffect(() => {
        if (!rawStats) return;
        //       set strength/constitution/dexterity/wisdom/charisma/intelligence from rawStats
        //       BUT recall: rawStats holds the FINAL stored value (raw, already +10'd),
        //       while the input field expects the ORIGINAL dice-roll number.
        setStrength(String(rawStats.strength - 10));
        setConstitution(String(rawStats.constitution - 10));
        setDexterity(String(rawStats.dexterity - 10));
        setWisdom(String(rawStats.wisdom - 10));
        setCharisma(String(rawStats.charisma - 10));
        setIntelligence(String(rawStats.intelligence - 10));
    }, [rawStats]);

    useEffect(() => {
        const pickCount = activeRaceStats?.freeStatPicks.length ?? 0;
        setPickAssignments(Array(pickCount).fill(null));
    }, [selectedRace?.id, selectedSubrace?.id]);

    useEffect(() => {
        if (!scifiCharacter || !activeRaceStats) return;

        const pickSourced = scifiCharacter.statBonuses.filter(
            (m) => (m.source === 'RacePick' || m.source === 'SubRacePick') && m.stat !== 'Unassigned'
        );

        const assignments: (string | null)[] = Array(activeRaceStats.freeStatPicks.length).fill(null);

        // pool: which slots are still unclaimed, remembering each slot's original index
        let pool = activeRaceStats.freeStatPicks.map((amount, index) => ({ amount, index }));

        pickSourced.forEach((pick) => {
            const poolIndex = pool.findIndex((slot) => slot.amount === pick.amount);
            if (poolIndex === -1) return; // no matching slot left, skip

            const slot = pool[poolIndex];
            assignments[slot.index] = pick.stat;

            // remove this slot from the pool so it can't be matched again
            pool = pool.filter((_, i) => i !== poolIndex);
        });

        setPickAssignments(assignments);
    }, [scifiCharacter, activeRaceStats]);

    return (
        <div className="page">
            <div className="app-header">
            <button onClick={() => navigate('/characters')} className="text-sm transition-colors hover:underline fg-muted">
                ← Back
            </button>
            <h1 className="text-lg font-semibold fg">{mode === 'add' ? 'Add Character' : mode === 'edit' ? 'Edit Character' : 'How did you get here?????? Seriously?? HOW DID YOU GET HERE???'}</h1>
            </div>

            <div className="max-w-md mx-auto px-6 py-8 lg:max-w-none lg:px-12 lg:py-10">
            <div className="rounded-2xl border p-6 flex flex-col gap-5 lg:p-10 lg:gap-8 bg-soft border-subtle">

                <div className="flex flex-col gap-5 lg:grid lg:grid-cols-3 lg:gap-6 lg:items-start">
                {/* Column 1: Identity + Stats */}
                <div className="flex flex-col gap-5">
                    {/* Identity section — name + level */}
                    <div className="form-section">
                        <h2 className="section-title">
                        Identity
                        </h2>

                        <div className="flex flex-col gap-1">
                        <label className="label">Name</label>
                        <input
                            type="text"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            className="field-input"
                            placeholder="Character name"
                        />
                        </div>

                        {/* Level input */}
                        <div className="flex flex-col gap-1">
                        <label className="label">Level (Currenly set to only 1)</label>
                        <input
                            type="number"
                            min="1"
                            max="20"
                            value={level}
                            disabled = {true}
                            onChange={(e) => setLevel(e.target.value)}
                            className="field-input"
                        />
                        </div>
                    </div>

                    {/* Stats section — ability scores + free stat picks */}
                    <div className="form-section">
                        <h2 className="section-title">
                            Stats
                        </h2>

                        {/* Stats inputs — strength, constitution, dexterity, wisdom, charisma, intelligence */}
                        <div className="flex flex-col gap-3">
                            {statFields.map(({ label, value, setValue }) => {
                            const racialEntry = activeRaceStats?.modifiers.find((m) => m.stat === label);
                            const isLocked = racialEntry?.isLocked ?? false;
                            const racialMod = getRacialModifierForStat(label);
                            const finalValue = isLocked ? racialMod : (parseInt(value) || 0) + racialMod + 10;

                            return (
                                <div key={label} className="flex flex-col gap-1">
                                <label className="label">{label}</label>

                                {isLocked ? (
                                    <span className="text-sm fg">
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
                                        onBlur={(e) => {
                                        const num = parseInt(e.target.value, 10);
                                        const clamped = isNaN(num) ? 1 : Math.min(10, Math.max(1, num));
                                        setValue(String(clamped));
                                        }}
                                        className="field-input w-16"
                                    />
                                    <span className="text-sm fg-muted">
                                        + {racialMod} (racial) + 10 = {finalValue}
                                    </span>
                                    </div>
                                )}
                                </div>
                            )
                            })}
                        </div>

                        {activeRaceStats && activeRaceStats.freeStatPicks.length > 0 && (
                            <div className="flex flex-col gap-4">
                            {activeRaceStats.freeStatPicks.map((amount, index) => (
                                <div key={index} className="flex flex-col gap-1">
                                <label className="label">
                                    Apply the +{amount} pick
                                </label>
                                <div className="flex flex-wrap gap-3">
                                    {getEligibleStatsForPick(index).map((stat) => (
                                    <label key={stat} className="flex items-center gap-1 text-sm">
                                        <input
                                        type="radio"
                                        name={`pick-${index}`}
                                        checked={pickAssignments[index] === stat}
                                        onChange={() => {
                                            setPickAssignments((prev) => {
                                            const newAssignments = [...prev];
                                            newAssignments[index] = stat;
                                            return newAssignments;
                                            });
                                        }}
                                        />
                                        {stat}
                                    </label>
                                    ))}
                                </div>
                                </div>
                            ))}
                            </div>
                        )}
                    </div>
                </div>

                {/* Column 2: Origin + Path */}
                <div className="flex flex-col gap-5">
                    {/* Origin section — race + subrace */}
                    <div className="form-section">
                        <h2 className="section-title">
                        Origin
                        </h2>

                        <div className="flex flex-col gap-1">
                        <label className="label">Race</label>
                        <select
                            value={raceId ?? ''}
                            onChange={(e) => {
                            const selectedId = e.target.value ? parseInt(e.target.value) : undefined
                            setRaceId(selectedId)
                            setSubraceId(undefined)
                            }}
                            className="field-input"
                        >
                            <option value="">Select a race</option>
                            {races?.map((race) => (
                            <option key={race.id} value={race.id}>
                                {race.name}
                            </option>
                            ))}
                        </select>
                        </div>

                        {showSubraceDropdown && (
                        <div className="flex flex-col gap-1">
                            <label className="label">Subrace</label>
                            <select
                            value={subraceId ?? ''}
                            onChange={(e) => setSubraceId(e.target.value ? parseInt(e.target.value) : undefined)}
                            className="field-input"
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
                    </div>

                    {/* Path section — class + background */}
                    <div className="form-section">
                        <h2 className="section-title">
                        Path
                        </h2>

                        <div className="flex flex-col gap-1">
                        <label className="label">Class</label>
                        <select
                            value={classId ?? ''}
                            onChange={(e) => setClassId(e.target.value ? parseInt(e.target.value) : undefined)}
                            className="field-input"
                        >
                            <option value="">Select a class</option>
                            {classes?.map((cls) => (
                            <option key={cls.id} value={cls.id}>
                                {cls.name}
                            </option>
                            ))}
                        </select>
                        </div>

                        <div className="flex flex-col gap-1">
                        <label className="label">Background</label>
                        <select
                            value={backgroundId ?? ''}
                            onChange={(e) => setBackgroundId(e.target.value ? parseInt(e.target.value) : undefined)}
                            className="field-input"
                        >
                            <option value="">Select a background</option>
                            {backgrounds?.map((bg) => (
                            <option key={bg.id} value={bg.id}>
                                {bg.name}
                            </option>
                            ))}
                        </select>
                        </div>
                    </div>
                </div>

                {/* Column 3: Notes */}
                <div className="form-section">
                    <h2 className="section-title">
                    Notes
                    </h2>
                    <CharacterNotes characterId={characterId} notes={notes} onNotesChange={setNotes} />
                </div>
                </div>

                {error && <p className="text-sm fg-danger">{error}</p>}

                <div className="flex gap-3 mt-2 lg:justify-end">
                <button onClick={() => navigate('/characters')} className="btn btn-neutral flex-1 lg:flex-none lg:px-8">
                    Cancel
                </button>
                <button onClick={handleSubmit} disabled={submitting} className="btn flex-1 lg:flex-none lg:px-8 btn-accent">
                    {submitting ? (mode === 'add' ? 'Creating…' : 'Saving…') : (mode === 'add' ? 'Create Character' : 'Save Character')}
                </button>
                </div>
            </div>
            </div>
        </div>
    )
}
