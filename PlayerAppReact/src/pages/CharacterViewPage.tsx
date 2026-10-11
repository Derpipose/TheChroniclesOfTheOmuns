import { useParams, useNavigate } from 'react-router-dom'
import { useCharacter, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useScifiBackgrounds, useScifiClasses, useScifiRaces, useScifiSubracesByRace } from '../hooks/useScifiReference'
import { useScifiStatsByCharacterId } from '../hooks/useScifiStats'
import { useEffect, useState } from 'react'
import { CharacterNotes } from '../components/CharacterNotes'

export default function CharacterViewPage() {
    const { id } = useParams()
    const navigate = useNavigate()

    const characterId = id ? parseInt(id, 10) : undefined

    const { data: character, isLoading: characterLoading } = useCharacter(characterId ?? 0)
    const { data: scifiCharacter, isLoading: scifiLoading } = useScifiCharacterByCharacterId(characterId ?? 0)
    const { data: adjustedStats, isLoading: adjustedStatsLoading } = useScifiStatsByCharacterId(characterId)

    const [notes, setNotes] = useState('')

    const { data: races } = useScifiRaces()
    const { data: subraces } = useScifiSubracesByRace(scifiCharacter?.scifiRaceId ?? 0)
    const { data: classes } = useScifiClasses()
    const { data: backgrounds } = useScifiBackgrounds()

    const race = races?.find((r) => r.id === scifiCharacter?.scifiRaceId)
    const subrace = subraces?.find((r) => r.id === scifiCharacter?.scifiSubraceId)
    const scifiClass = classes?.find((c) => c.id === scifiCharacter?.scifiClassId)
    const background = backgrounds?.find((b) => b.id === scifiCharacter?.scifiBackgroundId)
    const setting = "Scifi" // Placeholder for now, as there is only one setting currently.

    const statEntries = adjustedStats
        ? [
            { label: 'Strength', value: adjustedStats?.strength },
            { label: 'Constitution', value: adjustedStats?.constitution },
            { label: 'Dexterity', value: adjustedStats?.dexterity },
            { label: 'Wisdom', value: adjustedStats?.wisdom },
            { label: 'Charisma', value: adjustedStats?.charisma },
            { label: 'Intelligence', value: adjustedStats?.intelligence },
        ]
        : []

    useEffect(() => {
        if (!character) return;
        setNotes(character.notes)
    }, [character])

    if (characterLoading || scifiLoading || adjustedStatsLoading) {
        return (
        <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: 'var(--color-background)' }}>
            <p className="text-sm" style={{ color: 'var(--color-text-60)' }}>Loading character...</p>
        </div>
        )
    }

    if (!character) {
        return (
        <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: 'var(--color-background)' }}>
            <p className="text-sm" style={{ color: 'var(--color-text-60)' }}>Character not found.</p>
        </div>
        )
    }

    return (
        <div className="min-h-screen" style={{ backgroundColor: 'var(--color-background)' }}>
        <div className="app-header" style={{ backgroundColor: 'var(--color-background-60)' }}>
            <button onClick={() => navigate('/characters')} className="text-sm transition-colors hover:underline" style={{ color: 'var(--color-text-60)' }}>
            ← Back
            </button>
            <h1 className="text-lg font-semibold" style={{ color: 'var(--color-text)' }}>{character.name}</h1>
        </div>

        {/* Mobile layout (unchanged) */}
        <div className="lg:hidden max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

            {/* Overview card */}
            <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
            <div className="flex items-center justify-between">
                <span className="text-xs font-medium uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>{character.name}</span>
            </div>
            <div>
                <span className="text-xs font-medium px-2.5 py-1 rounded-full"
                style={{
                    backgroundColor: character.isActive ? '#E4EEE4' : 'var(--color-primary-30)',
                    color: character.isActive ? '#3D5C3D' : 'var(--color-text)',
                }}>{setting}</span>
                <span
                className="text-xs font-medium px-2.5 py-1 rounded-full"
                style={{
                    backgroundColor: character.isActive ? '#E4EEE4' : 'var(--color-primary-30)',
                    color: character.isActive ? '#3D5C3D' : 'var(--color-text)',
                }}
                >
                {character.isActive ? 'Active' : 'In Edit Mode'}
                </span>
            </div>
            <div className="flex flex-wrap items-center gap-3">
                <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}>
                Level {character.level}
                </span>
                <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}>
                {character.currentHealth} HP
                </span>
                <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}>
                AC {adjustedStats?.armorClass ?? '—'}
                </span>
                <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: 'var(--color-primary-30)', color: 'var(--color-text)' }}>
                DR {adjustedStats?.damageResist ?? '—'}
                </span>
            </div>
            </div>

            {/* Scifi details card */}
            {scifiCharacter && (
            <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                <h2 className="text-sm font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Details</h2>
                <div className="flex flex-col gap-2 text-sm" style={{ color: 'var(--color-text)' }}>
                <p><span style={{ color: 'var(--color-text-60)' }}>Race:</span> {race?.name ?? '—'}</p>
                {scifiCharacter.scifiSubraceId && (
                    <p><span style={{ color: 'var(--color-text-60)' }}>Subrace:</span> {subrace?.subRaceName ?? '—'}</p>
                )}
                <p><span style={{ color: 'var(--color-text-60)' }}>Class:</span> {scifiClass?.name ?? '—'}</p>
                <p><span style={{ color: 'var(--color-text-60)' }}>Background:</span> {background?.name ?? '—'}</p>
                </div>
            </div>
            )}

            {/* Stats card */}
            { adjustedStats && (
            <div className="rounded-2xl border p-6" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                <h2 className="text-sm font-semibold uppercase tracking-wide mb-3" style={{ color: 'var(--color-text-60)' }}>Stats</h2>
                <div className="grid grid-cols-2 gap-3">
                {statEntries.map(({ label, value }) => (
                    <div key={label} className="flex items-center justify-between rounded-lg px-3 py-2" style={{ backgroundColor: 'var(--color-primary-30)' }}>
                    <span className="text-xs font-medium uppercase tracking-wide" style={{ color: 'var(--color-text)' }}>{label}</span>
                    <span className="text-sm font-semibold" style={{ color: 'var(--color-text)' }}>{value}</span>
                    </div>
                ))}
                </div>
            </div>
            )}

            <div className="flex gap-3">
            <button
                onClick={() => navigate(`/character/${characterId}/edit`)}
                className="flex-1 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
                style={{ backgroundColor: 'var(--color-accent)', color: 'var(--color-text-30)' }}
            >
                Edit Character
            </button>
            </div>

            {/* Notes card */}
            <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                <h2 className="text-sm font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Notes</h2>
                <CharacterNotes characterId={characterId} notes={notes} onNotesChange={setNotes} />
            </div>
        </div>

        {/* Desktop layout */}
        <div className="hidden lg:flex lg:flex-col gap-6 max-w-6xl mx-auto px-10 py-10">

            {/* Hero */}
            <div className="rounded-2xl border p-6 flex items-center justify-between gap-6" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                <div className="flex items-center gap-5">
                    <div
                        className="w-20 h-20 rounded-xl border-2 flex items-center justify-center text-3xl font-bold shrink-0"
                        style={{ backgroundColor: 'var(--color-primary-30)', borderColor: 'var(--color-primary-60)', color: 'var(--color-text)' }}
                    >
                        {character.name.charAt(0).toUpperCase()}
                    </div>
                    <div className="flex flex-col gap-1">
                        <h2 className="text-3xl font-bold" style={{ color: 'var(--color-text)' }}>{character.name}</h2>
                        <p className="text-sm" style={{ color: 'var(--color-text-60)' }}>
                            {[race?.name, scifiClass?.name, `Level ${character.level}`].filter(Boolean).join(' • ')}
                        </p>
                        {background && (
                            <p className="text-sm" style={{ color: 'var(--color-text-60)' }}>
                                Background: <span style={{ color: 'var(--color-text)' }}>{background.name}</span>
                            </p>
                        )}
                    </div>
                </div>
                <div className="flex items-center gap-3 shrink-0">
                    <span className="text-xs font-medium px-2.5 py-1 rounded-full"
                        style={{
                            backgroundColor: character.isActive ? '#E4EEE4' : 'var(--color-primary-30)',
                            color: character.isActive ? '#3D5C3D' : 'var(--color-text)',
                        }}>{setting}</span>
                    <span className="text-xs font-medium px-2.5 py-1 rounded-full"
                        style={{
                            backgroundColor: character.isActive ? '#E4EEE4' : 'var(--color-primary-30)',
                            color: character.isActive ? '#3D5C3D' : 'var(--color-text)',
                        }}>{character.isActive ? 'Active' : 'In Edit Mode'}</span>
                    <button
                        onClick={() => navigate(`/character/${characterId}/edit`)}
                        className="rounded-lg px-5 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
                        style={{ backgroundColor: 'var(--color-accent)', color: 'var(--color-text-30)' }}
                    >
                        Edit Character
                    </button>
                </div>
            </div>

            <div className="grid grid-cols-3 gap-6 items-start">
                {/* Column 1: Vitals + Stats (unchanged) */}
                <div className="flex flex-col gap-6">
                    {adjustedStats && (
                        <div className="rounded-2xl border p-6 flex flex-col gap-4" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                            <div className="grid grid-cols-2 gap-3">
                                <div className="flex flex-col gap-3">
                                    <div className="rounded-xl border flex flex-col items-center justify-center gap-1 py-2" style={{ backgroundColor: 'var(--color-primary-30)', borderColor: 'var(--color-primary-60)' }}>
                                        <span className="text-xs font-medium uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>AC</span>
                                        <span className="text-xl font-bold" style={{ color: 'var(--color-text)' }}>{adjustedStats.armorClass}</span>
                                    </div>
                                    <div className="rounded-xl border flex flex-col items-center justify-center gap-1 py-2" style={{ backgroundColor: 'var(--color-primary-30)', borderColor: 'var(--color-primary-60)' }}>
                                        <span className="text-xs font-medium uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>DR</span>
                                        <span className="text-xl font-bold" style={{ color: 'var(--color-text)' }}>{adjustedStats.damageResist}</span>
                                    </div>
                                </div>
                                <div className="rounded-xl border flex flex-col items-center justify-center gap-1 py-4" style={{ backgroundColor: 'var(--color-primary-30)', borderColor: 'var(--color-primary-60)' }}>
                                    <span className="text-xs font-medium uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>HP</span>
                                    <span className="text-2xl font-bold" style={{ color: 'var(--color-text)' }}>{adjustedStats.currentHealth} / {adjustedStats.maxHealth}</span>
                                </div>
                            </div>
                            <div className="rounded-full h-2 overflow-hidden" style={{ backgroundColor: 'var(--color-primary-30)' }}>
                                <div
                                    className="h-full rounded-full"
                                    style={{
                                        width: `${adjustedStats.currentHealth > 0 ? Math.min(100, (adjustedStats.currentHealth / adjustedStats.maxHealth) * 100) : 0}%`,
                                        backgroundColor: '#4C8B4C',
                                    }}
                                />
                            </div>
                        </div>
                    )}

                    {adjustedStats && (
                        <div className="rounded-2xl border p-6" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                            <h2 className="text-xs font-semibold uppercase tracking-wide mb-3" style={{ color: 'var(--color-text-60)' }}>Ability Scores</h2>
                            <div className="grid grid-cols-3 gap-3">
                                {statEntries.map(({ label, value }) => (
                                    <div key={label} className="rounded-xl border flex flex-col items-center justify-center gap-1 py-3" style={{ backgroundColor: 'var(--color-primary-30)', borderColor: 'var(--color-primary-60)' }}>
                                        <span className="text-[10px] font-medium uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>{label.slice(0, 3)}</span>
                                        <span className="text-lg font-bold" style={{ color: 'var(--color-text)' }}>{value}</span>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>

                {/* Column 2: Racial traits + Class Features + Background */}
                <div className="flex flex-col gap-6">
                    {race && (
                        <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                            <h2 className="text-xs font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Racial Traits</h2>
                            <div>
                                {race.description ? (
                                    <details className="group">
                                        <summary className="collapse-summary">
                                            <span className="transition-transform group-open:rotate-90" style={{ color: 'var(--color-text-60)' }}>›</span>
                                            {race.name}
                                        </summary>
                                        <p className="text-sm mt-1 pl-4" style={{ color: 'var(--color-text-60)' }}>{race.description}</p>
                                    </details>
                                ) : (
                                    <p className="text-sm font-semibold" style={{ color: 'var(--color-text)' }}>{race.name}</p>
                                )}
                            </div>
                            {subrace && (
                                <div className="pt-3 border-t" style={{ borderColor: 'var(--color-primary-30)' }}>
                                    {subrace.description ? (
                                        <details className="group">
                                            <summary className="collapse-summary">
                                                <span className="transition-transform group-open:rotate-90" style={{ color: 'var(--color-text-60)' }}>›</span>
                                                {subrace.subRaceName}
                                            </summary>
                                            <p className="text-sm mt-1 pl-4" style={{ color: 'var(--color-text-60)' }}>{subrace.description}</p>
                                        </details>
                                    ) : (
                                        <p className="text-sm font-semibold" style={{ color: 'var(--color-text)' }}>{subrace.subRaceName}</p>
                                    )}
                                </div>
                            )}
                        </div>
                    )}

                    {scifiClass && (
                        <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                            <h2 className="text-xs font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Class</h2>
                            <div>
                                <p className="text-sm font-semibold" style={{ color: 'var(--color-text)' }}>{scifiClass.name}</p>
                                {scifiClass.description && <p className="text-sm mt-1" style={{ color: 'var(--color-text-60)' }}>{scifiClass.description}</p>}
                            </div>
                        </div>
                    )}

                    {background && (
                        <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                            <h2 className="text-xs font-semibold uppercase tracking-wide" style={{ color: 'var(--color-text-60)' }}>Background</h2>
                            <div>
                                {background.description ? (
                                    <details className="group">
                                        <summary className="collapse-summary">
                                            <span className="transition-transform group-open:rotate-90" style={{ color: 'var(--color-text-60)' }}>›</span>
                                            {background.name}
                                        </summary>
                                        <p className="text-sm mt-1 pl-4" style={{ color: 'var(--color-text-60)' }}>{background.description}</p>
                                    </details>
                                ) : (
                                    <p className="text-sm font-semibold" style={{ color: 'var(--color-text)' }}>{background.name}</p>
                                )}
                            </div>
                        </div>
                    )}
                </div>

                {/* Column 3: Notes */}
                <div className="flex flex-col gap-6">
                    <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: 'var(--color-background-60)', borderColor: 'var(--color-primary-60)' }}>
                        <CharacterNotes characterId={characterId} notes={notes} onNotesChange={setNotes} />
                    </div>
                </div>
            </div>
        </div>
        </div>
    )
}
