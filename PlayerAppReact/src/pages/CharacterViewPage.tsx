import { useParams, useNavigate } from 'react-router-dom'
import { useCharacter, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useScifiBackgrounds, useScifiClasses, useScifiRaces, useScifiSubracesByRace } from '../hooks/useScifiReference'
import { useScifiStatsByCharacterId } from '../hooks/useScifiStats'
import { useEffect, useState } from 'react'
import { CharacterNotes } from '../components/CharacterNotes'
import PanelCard from '../components/PanelCard'
import { ScrollText } from 'lucide-react'
import AbilityScoresCard from '../components/CharacterViewPage/AbilityScoresCard'
import CenteredMessage from '../components/CharacterViewPage/CenteredMessage'
import DetailsCard from '../components/CharacterViewPage/DetailsCard'
import CharacterHeader from '../components/CharacterViewPage/CharacterHeader'
import OverviewCard from '../components/CharacterViewPage/OverviewCard'
import StatListCard from '../components/CharacterViewPage/StatListCard'
import { BackgroundCard, ClassCard, RacialTraitsCard } from '../components/CharacterViewPage/TraitCards'
import VitalsCard from '../components/CharacterViewPage/VitalsCard'

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
                <OverviewCard character={character} adjustedStats={adjustedStats} setting={setting} />

                {scifiCharacter && (
                    <DetailsCard
                        race={race?.name}
                        subrace={subrace?.subRaceName}
                        showSubrace={Boolean(scifiCharacter.scifiSubraceId)}
                        characterClass={scifiClass?.name}
                        background={background?.name}
                    />
                )}

                {adjustedStats && <StatListCard statEntries={statEntries} />}

                <div className="flex gap-3">
                    <button onClick={goToEdit} className="btn btn-accent flex-1">Edit Character</button>
                </div>

                <PanelCard title="Notes" icon={<ScrollText />} collapsible>
                    {notesEditor}
                </PanelCard>
            </div>

            {/* Desktop layout */}
            <div className="hidden lg:flex lg:flex-col gap-6 max-w-6xl mx-auto px-10 py-10">
                <CharacterHeader
                    character={character}
                    raceName={race?.name}
                    characterClass={scifiClass?.name}
                    background={background}
                    setting={setting}
                    onEdit={goToEdit}
                />

                <div className="grid grid-cols-3 gap-6 items-start">
                    {/* Column 1: Vitals + Stats */}
                    <div className="flex flex-col gap-6">
                        {adjustedStats && <VitalsCard adjustedStats={adjustedStats} />}
                        {adjustedStats && <AbilityScoresCard statEntries={statEntries} />}
                    </div>

                    {/* Column 2: Racial traits + Class + Background */}
                    <div className="flex flex-col gap-6">
                        {race && <RacialTraitsCard race={race} subrace={subrace} />}
                        {scifiClass && <ClassCard scifiClass={scifiClass} />}
                        {background && <BackgroundCard background={background} />}
                    </div>

                    {/* Column 3: Notes */}
                    <div className="flex flex-col gap-6">
                        <PanelCard title="Notes" icon={<ScrollText />} collapsible>
                            {notesEditor}
                        </PanelCard>
                    </div>
                </div>
            </div>
        </div>
    )
}
