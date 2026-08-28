import { useParams, useNavigate } from 'react-router-dom'
import { useCharacter, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useScifiBackgrounds, useScifiClasses, useScifiRaces, useScifiSubracesByRace } from '../hooks/useScifiReference'
import { useScifiStatsByCharacterId } from '../hooks/useScifiStats'

export default function CharacterViewPage() {
    const { id } = useParams()
    const navigate = useNavigate()

    const characterId = id ? parseInt(id, 10) : undefined

    const { data: character, isLoading: characterLoading } = useCharacter(characterId ?? 0)
    const { data: scifiCharacter, isLoading: scifiLoading } = useScifiCharacterByCharacterId(characterId ?? 0)
    const { data: adjustedStats, isLoading: adjustedStatsLoading } = useScifiStatsByCharacterId(characterId)

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

    if (characterLoading || scifiLoading || adjustedStatsLoading) {
        return (
        <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: '#F5F0E8' }}>
            <p className="text-sm" style={{ color: '#7A6F5C' }}>Loading character...</p>
        </div>
        )
    }

    if (!character) {
        return (
        <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: '#F5F0E8' }}>
            <p className="text-sm" style={{ color: '#7A6F5C' }}>Character not found.</p>
        </div>
        )
    }

    return (
        <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
        <div className="border-b px-6 py-4 flex items-center gap-3" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
            <button onClick={() => navigate('/characters')} className="text-sm transition-colors hover:underline" style={{ color: '#7A6F5C' }}>
            ← Back
            </button>
            <h1 className="text-lg font-semibold" style={{ color: '#2C2416' }}>{character.name}</h1>
        </div>

        <div className="max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

            {/* Overview card */}
            <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
            <div className="flex items-center justify-between">
                <span className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>{character.name}</span>
            </div>
            <div>
                <span className="text-xs font-medium px-2.5 py-1 rounded-full"
                style={{
                    backgroundColor: character.active ? '#E4EEE4' : '#EDE8DC',
                    color: character.active ? '#3D5C3D' : '#5C4F38',
                }}>{setting}</span>
                <span
                className="text-xs font-medium px-2.5 py-1 rounded-full"
                style={{
                    backgroundColor: character.active ? '#E4EEE4' : '#EDE8DC',
                    color: character.active ? '#3D5C3D' : '#5C4F38',
                }}
                >
                {character.active ? 'Active' : 'In Edit Mode'}
                </span>
            </div>
            <div className="flex items-center gap-3">
                <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
                Level {character.level}
                </span>
                <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
                {character.maxHealth} HP
                </span>
            </div>
            </div>

            {/* Scifi details card */}
            {scifiCharacter && (
            <div className="rounded-2xl border p-6 flex flex-col gap-3" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
                <h2 className="text-sm font-semibold uppercase tracking-wide" style={{ color: '#7A6F5C' }}>Details</h2>
                <div className="flex flex-col gap-2 text-sm" style={{ color: '#2C2416' }}>
                <p><span style={{ color: '#7A6F5C' }}>Race:</span> {race?.name ?? '—'}</p>
                {scifiCharacter.scifiSubraceId && (
                    <p><span style={{ color: '#7A6F5C' }}>Subrace:</span> {subrace?.subRaceName ?? '—'}</p>
                )}
                <p><span style={{ color: '#7A6F5C' }}>Class:</span> {scifiClass?.name ?? '—'}</p>
                <p><span style={{ color: '#7A6F5C' }}>Background:</span> {background?.name ?? '—'}</p>
                </div>
            </div>
            )}

            {/* Stats card */}
            { adjustedStats && (
            <div className="rounded-2xl border p-6" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
                <h2 className="text-sm font-semibold uppercase tracking-wide mb-3" style={{ color: '#7A6F5C' }}>Stats</h2>
                <div className="grid grid-cols-2 gap-3">
                {statEntries.map(({ label, value }) => (
                    <div key={label} className="flex items-center justify-between rounded-lg px-3 py-2" style={{ backgroundColor: '#EDE8DC' }}>
                    <span className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>{label}</span>
                    <span className="text-sm font-semibold" style={{ color: '#2C2416' }}>{value}</span>
                    </div>
                ))}
                </div>
            </div>
            )}

            <div className="flex gap-3">
            <button
                onClick={() => navigate(`/character/${characterId}/edit`)}
                className="flex-1 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
                style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
            >
                Edit Character
            </button>
            </div>
        </div>
        </div>
    )
}