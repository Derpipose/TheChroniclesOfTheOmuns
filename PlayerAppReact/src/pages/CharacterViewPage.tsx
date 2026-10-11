import { useParams, useNavigate } from 'react-router-dom'
import { useCharacter, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useScifiBackgrounds, useScifiClasses, useScifiRaces, useScifiSubracesByRace } from '../hooks/useScifiReference'
import { useScifiStatsByCharacterId } from '../hooks/useScifiStats'
import { useEffect, useState, type ReactNode } from 'react'
import { CharacterNotes } from '../components/CharacterNotes'
import Card from '../components/Card'
import Pill from '../components/Pill'

const EDIT_BTN = 'btn btn-accent'

function CenteredMessage({ children }: { children: ReactNode }) {
    return (
        <div className="page flex items-center justify-center">
            <p className="text-sm fg-muted">{children}</p>
        </div>
    )
}

function StatTile({ label, value, py = 'py-3', labelSize = 'text-xs', valueSize = 'text-lg' }: {
    label: string
    value: ReactNode
    py?: string
    labelSize?: string
    valueSize?: string
}) {
    return (
        <div className={`stat-tile ${py}`}>
            <span className={`stat-tile-label ${labelSize}`}>{label}</span>
            <span className={`stat-tile-value ${valueSize}`}>{value}</span>
        </div>
    )
}

function DetailRow({ label, value }: { label: string; value?: string | null }) {
    return <p><span className="fg-muted">{label}:</span> {value ?? '—'}</p>
}

/** A name that expands to show its description, or plain text when there is none. */
function Expandable({ name, description }: { name: string; description?: string | null }) {
    if (!description) return <p className="text-sm font-semibold fg">{name}</p>

    return (
        <details className="group">
            <summary className="collapse-summary">
                <span className="fg-muted transition-transform group-open:rotate-90">›</span>
                {name}
            </summary>
            <p className="text-sm mt-1 pl-4 fg-muted">{description}</p>
        </details>
    )
}

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
        return <CenteredMessage>Loading character...</CenteredMessage>
    }

    if (!character) {
        return <CenteredMessage>Character not found.</CenteredMessage>
    }

    const goToEdit = () => navigate(`/character/${characterId}/edit`)
    const healthPercent = adjustedStats && adjustedStats.currentHealth > 0
        ? Math.min(100, (adjustedStats.currentHealth / adjustedStats.maxHealth) * 100)
        : 0
    const notesEditor = <CharacterNotes characterId={characterId} notes={notes} onNotesChange={setNotes} />

    return (
        <div className="page">
        <div className="app-header bg-soft">
            <button onClick={() => navigate('/characters')} className="text-sm transition-colors hover:underline fg-muted">
            ← Back
            </button>
            <h1 className="text-lg font-semibold fg">{character.name}</h1>
        </div>

        {/* Mobile layout */}
        <div className="lg:hidden max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

            {/* Overview card */}
            <Card className="flex flex-col gap-3">
                <span className="card-title font-medium">{character.name}</span>
                <div>
                    <Pill variant={character.isActive ? 'active' : 'neutral'}>{setting}</Pill>
                    <Pill variant={character.isActive ? 'active' : 'neutral'}>{character.isActive ? 'Active' : 'In Edit Mode'}</Pill>
                </div>
                <div className="flex flex-wrap items-center gap-3">
                    <Pill>Level {character.level}</Pill>
                    <Pill>{character.currentHealth} HP</Pill>
                    <Pill>AC {adjustedStats?.armorClass ?? '—'}</Pill>
                    <Pill>DR {adjustedStats?.damageResist ?? '—'}</Pill>
                </div>
            </Card>

            {/* Scifi details card */}
            {scifiCharacter && (
                <Card title="Details" className="flex flex-col gap-3">
                    <div className="flex flex-col gap-2 text-sm fg">
                        <DetailRow label="Race" value={race?.name} />
                        {scifiCharacter.scifiSubraceId && <DetailRow label="Subrace" value={subrace?.subRaceName} />}
                        <DetailRow label="Class" value={scifiClass?.name} />
                        <DetailRow label="Background" value={background?.name} />
                    </div>
                </Card>
            )}

            {/* Stats card */}
            {adjustedStats && (
                <Card title="Stats">
                    <div className="grid grid-cols-2 gap-3 mt-3">
                        {statEntries.map(({ label, value }) => (
                            <div key={label} className="row-chip">
                                <span className="text-xs font-medium uppercase tracking-wide fg">{label}</span>
                                <span className="text-sm font-semibold fg">{value}</span>
                            </div>
                        ))}
                    </div>
                </Card>
            )}

            <div className="flex gap-3">
                <button onClick={goToEdit} className={`${EDIT_BTN} flex-1`}>Edit Character</button>
            </div>

            {/* Notes card */}
            <Card title="Notes" className="flex flex-col gap-3">
                {notesEditor}
            </Card>
        </div>

        {/* Desktop layout */}
        <div className="hidden lg:flex lg:flex-col gap-6 max-w-6xl mx-auto px-10 py-10">

            {/* Hero */}
            <Card className="flex items-center justify-between gap-6">
                <div className="flex items-center gap-5">
                    <div
                        className="avatar"
                    >
                        {character.name.charAt(0).toUpperCase()}
                    </div>
                    <div className="flex flex-col gap-1">
                        <h2 className="text-3xl font-bold fg">{character.name}</h2>
                        <p className="text-sm fg-muted">
                            {[race?.name, scifiClass?.name, `Level ${character.level}`].filter(Boolean).join(' • ')}
                        </p>
                        {background && (
                            <p className="text-sm fg-muted">
                                Background: <span className="fg">{background.name}</span>
                            </p>
                        )}
                    </div>
                </div>
                <div className="flex items-center gap-3 shrink-0">
                    <Pill variant={character.isActive ? 'active' : 'neutral'}>{setting}</Pill>
                    <Pill variant={character.isActive ? 'active' : 'neutral'}>{character.isActive ? 'Active' : 'In Edit Mode'}</Pill>
                    <button onClick={goToEdit} className={`${EDIT_BTN} px-5`}>Edit Character</button>
                </div>
            </Card>

            <div className="grid grid-cols-3 gap-6 items-start">
                {/* Column 1: Vitals + Stats */}
                <div className="flex flex-col gap-6">
                    {adjustedStats && (
                        <Card className="flex flex-col gap-4">
                            <div className="grid grid-cols-2 gap-3">
                                <div className="flex flex-col gap-3">
                                    <StatTile label="AC" value={adjustedStats.armorClass} py="py-2" valueSize="text-xl" />
                                    <StatTile label="DR" value={adjustedStats.damageResist} py="py-2" valueSize="text-xl" />
                                </div>
                                <StatTile
                                    label="HP"
                                    value={`${adjustedStats.currentHealth} / ${adjustedStats.maxHealth}`}
                                    py="py-4"
                                    valueSize="text-2xl"
                                />
                            </div>
                            <div className="progress">
                                <div className="progress-fill" style={{ width: `${healthPercent}%` }} />
                            </div>
                        </Card>
                    )}

                    {adjustedStats && (
                        <Card title="Ability Scores">
                            <div className="grid grid-cols-3 gap-3 mt-3">
                                {statEntries.map(({ label, value }) => (
                                    <StatTile key={label} label={label.slice(0, 3)} value={value} labelSize="text-[10px]" />
                                ))}
                            </div>
                        </Card>
                    )}
                </div>

                {/* Column 2: Racial traits + Class Features + Background */}
                <div className="flex flex-col gap-6">
                    {race && (
                        <Card title="Racial Traits" className="flex flex-col gap-3">
                            <Expandable name={race.name} description={race.description} />
                            {subrace && (
                                <div className="pt-3 border-t border-subtle">
                                    <Expandable name={subrace.subRaceName} description={subrace.description} />
                                </div>
                            )}
                        </Card>
                    )}

                    {scifiClass && (
                        <Card title="Class" className="flex flex-col gap-3">
                            <div>
                                <p className="text-sm font-semibold fg">{scifiClass.name}</p>
                                {scifiClass.description && <p className="text-sm mt-1 fg-muted">{scifiClass.description}</p>}
                            </div>
                        </Card>
                    )}

                    {background && (
                        <Card title="Background" className="flex flex-col gap-3">
                            <Expandable name={background.name} description={background.description} />
                        </Card>
                    )}
                </div>

                {/* Column 3: Notes */}
                <div className="flex flex-col gap-6">
                    <Card className="flex flex-col gap-3">
                        {notesEditor}
                    </Card>
                </div>
            </div>
        </div>
        </div>
    )
}
