
import { useState } from 'react'
// import { useNavigate } from 'react-router-dom'
import { useCharacters, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import { useScifiWeapons } from '../hooks/useScifiWeapon'
import {
    useCreateScifiWeaponInstance,
    useScifiWeaponInstances,
} from '../hooks/useScifiWeaponInstance'
import NavBar from '../components/NavBar'

export default function AddWeaponInstancePage() {
    // const navigate = useNavigate()

    const [selectedCharacterId, setSelectedCharacterId] = useState<number>(0)
    const [selectedWeaponId, setSelectedWeaponId] = useState<number>(0)

    const { data: characters, isLoading: charactersLoading } = useCharacters()
    const { data: weapons, isLoading: weaponsLoading } = useScifiWeapons()
    const { data: selectedScifiCharacter, isLoading: scifiCharacterLoading } =
        useScifiCharacterByCharacterId(selectedCharacterId)


    const [weaponSearch, setWeaponSearch] = useState('')
    const [weaponDropdownOpen, setWeaponDropdownOpen] = useState(false)

    const filteredWeapons = weapons?.filter((weapon) =>
        weapon.name
            .toLowerCase()
            .includes(weaponSearch.toLowerCase())
    ) ?? []

    const {
        data: existingWeapons,
        isLoading: existingWeaponsLoading,
    } = useScifiWeaponInstances(selectedScifiCharacter?.id ?? 0)

    const createWeaponInstance = useCreateScifiWeaponInstance()

    const selectedCharacter = characters?.find(
        (character) => character.id === selectedCharacterId
    )

    const selectedWeapon = weapons?.find(
        (weapon) => weapon.id === selectedWeaponId
    )

    const handleAddWeapon = () => {
    if (!selectedCharacterId || !selectedWeaponId) {
        return
    }



    if (!selectedScifiCharacter) {
        return
    }

    createWeaponInstance.mutate({
        scifiWeaponId: selectedWeaponId,
        scifiCharacterId: selectedScifiCharacter.id,
        scifiWeaponBlueprintId: selectedWeaponId,
    })
}

    if (charactersLoading || weaponsLoading) {
        return (
            <div
                className="page flex items-center justify-center"
            >
                <p
                    className="text-sm fg-muted"
                >
                    Loading...
                </p>
            </div>
        )
    }

    return (
        <div
            className="page"
        >
                <NavBar title="Edit Weapon" />


            <div className="max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

                {/* Add weapon card */}
                <div
                    className="rounded-2xl border p-6 flex flex-col gap-4 panel"
                >
                    <h2
                        className="card-title"
                    >
                        Add Weapon
                    </h2>

                    {/* Character selector */}
                    <div className="flex flex-col gap-2">
                        <label
                            htmlFor="character"
                            className="label"
                        >
                            Character
                        </label>

                        <select
                            id="character"
                            value={selectedCharacterId}
                            onChange={(e) => setSelectedCharacterId(Number(e.target.value))}
                            className="field-input"
                        >
                            <option value={0}>Select a character...</option>

                            {characters?.map((character) => (
                                <option key={character.id} value={character.id}>
                                    {character.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    {/* Weapon selector */}
                    <div className="flex flex-col gap-2">
                        <label
                            htmlFor="weapon"
                            className="label"
                        >
                            Weapon
                        </label>

                        <div className="relative">
    <button
        type="button"
        onClick={() => setWeaponDropdownOpen((open) => !open)}
        className={`field-input w-full text-left ${selectedWeaponId ? '' : 'fg-muted'}`}
    >
        {selectedWeaponId
            ? weapons?.find((weapon) => weapon.id === selectedWeaponId)?.name
            : 'Select a weapon...'}
    </button>

    {weaponDropdownOpen && (
        <div
            className="absolute z-10 mt-1 w-full rounded-lg border p-2 shadow-sm panel"
        >
            <input
                type="text"
                value={weaponSearch}
                onChange={(e) => setWeaponSearch(e.target.value)}
                placeholder="Search weapons..."
                autoFocus
                className="field-input w-full"
            />

            <div className="mt-2 max-h-60 overflow-y-auto">
                {filteredWeapons.length === 0 ? (
                    <p
                        className="px-3 py-2 text-sm fg-muted"
                    >
                        No weapons found.
                    </p>
                ) : (
                    filteredWeapons.map((weapon) => (
                        <button
                            key={weapon.id}
                            type="button"
                            onClick={() => {
                                setSelectedWeaponId(weapon.id)
                                setWeaponSearch('')
                                setWeaponDropdownOpen(false)
                            }}
                            className={`w-full rounded-lg px-3 py-2 text-left text-sm transition-colors hover:opacity-80 fg ${selectedWeaponId === weapon.id ? 'bg-soft' : ''}`}
                        >
                            {weapon.name}
                        </button>
                    ))
                )}
            </div>
        </div>
    )}
</div>
                    </div>

                    {/* Selected weapon preview */}
                    {selectedWeapon && (
                        <div
                            className="rounded-lg p-3 flex flex-col gap-1 bg-soft"
                        >
                            <span
                                className="label"
                            >
                                Selected Weapon
                            </span>

                            <span
                                className="text-sm font-semibold fg"
                            >
                                {selectedWeapon.name}
                            </span>

                            <span
                                className="text-xs fg-muted"
                            >
                                {selectedWeapon.damageFormula} •{' '}
                                {selectedWeapon.damageType} •{' '}
                                {selectedWeapon.rangeCategory}
                            </span>
                        </div>
                    )}

                    <button
                        onClick={handleAddWeapon}
                        disabled={
                            !selectedCharacterId ||
                            !selectedWeaponId ||
                            scifiCharacterLoading ||
                            !selectedScifiCharacter ||
                            createWeaponInstance.isPending
                        }
                        className="btn btn-accent"
                    >
                        {createWeaponInstance.isPending
                            ? 'Adding...'
                            : 'Add Weapon'}
                    </button>

                    {createWeaponInstance.isError && (
                        <p
                            className="text-xs fg-danger"
                        >
                            Failed to add weapon. Please try again.
                        </p>
                    )}

                    {createWeaponInstance.isSuccess && (
                        <p
                            className="text-xs fg-success"
                        >
                            {selectedWeapon?.name ?? 'Weapon'} added to{' '}
                            {selectedCharacter?.name ?? 'character'}.
                        </p>
                    )}
                </div>

                {/* Existing weapons */}
                {selectedCharacterId > 0 && (
    <div
        className="rounded-2xl border p-6 flex flex-col gap-3 panel"
    >
        <div className="flex items-center justify-between">
            <h2
                className="card-title"
            >
                {selectedCharacter?.name ?? 'Character'}'s Weapons
            </h2>

            <span
                className="text-xs fg-muted"
            >
                {existingWeapons?.length ?? 0}
            </span>
        </div>

        {existingWeaponsLoading ? (
            <p
                className="text-sm fg-muted"
            >
                Loading weapons...
            </p>
        ) : existingWeapons?.length === 0 ? (
            <p
                className="text-sm fg-muted"
            >
                This character has no weapons.
            </p>
        ) : (
            <div className="flex flex-col gap-2">
                {existingWeapons?.map((weapon) => (
                    <details
                        key={weapon.id}
                        className="rounded-lg overflow-hidden bg-soft"
                    >
                        <summary
                            className="list-none [&::-webkit-details-marker]:hidden px-3 py-3 flex items-center justify-between cursor-pointer fg"
                        >
                            <div className="flex items-center gap-2">

                                <div className="flex flex-col">
                                    <span className="text-sm font-semibold">
                                        {weapon.nickname || weapon.name}
                                    </span>

                                    {weapon.nickname &&
                                        weapon.nickname !== weapon.name && (
                                            <span
                                                className="text-xs fg-muted"
                                            >
                                                {weapon.name}
                                            </span>
                                        )}
                                </div>
                            </div>

                            <div className="flex items-center gap-2">
                                {weapon.favorite && (
                                    <span
                                        className="text-xs"
                                        title="Favorite"
                                    >
                                        ★
                                    </span>
                                )}

                                <span
                                    className="text-xs fg-muted"
                                >
                                    {weapon.currentAmmo}/{weapon.ammoCapacity}
                                </span>
                            </div>
                        </summary>

                        <div
                            className="border-t px-3 py-3 flex flex-col gap-3 border-subtle"
                        >
                            <div className="grid grid-cols-2 gap-2">
                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide fg-muted"
                                    >
                                        Damage
                                    </span>
                                    <p
                                        className="text-sm font-semibold fg"
                                    >
                                        {weapon.damageFormula}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide fg-muted"
                                    >
                                        Damage Type
                                    </span>
                                    <p
                                        className="text-sm font-semibold fg"
                                    >
                                        {weapon.damageType}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide fg-muted"
                                    >
                                        Modifier
                                    </span>
                                    <p
                                        className="text-sm font-semibold fg"
                                    >
                                        {weapon.modifierStat}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide fg-muted"
                                    >
                                        Range
                                    </span>
                                    <p
                                        className="text-sm font-semibold fg"
                                    >
                                        {weapon.rangeCategory}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide fg-muted"
                                    >
                                        Rate of Fire
                                    </span>
                                    <p
                                        className="text-sm font-semibold fg"
                                    >
                                        {weapon.rateOfFire}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide fg-muted"
                                    >
                                        Reload
                                    </span>
                                    <p
                                        className="text-sm font-semibold fg"
                                    >
                                        {weapon.reloadTimeInActions} actions
                                    </p>
                                </div>
                            </div>

                            <div
                                className="border-t pt-3 border-subtle"
                            >
                                <span
                                    className="label"
                                >
                                    Critical
                                </span>

                                <div className="grid grid-cols-3 gap-2 mt-2">
                                    <div>
                                        <span
                                            className="text-xs fg-muted"
                                        >
                                            Range
                                        </span>
                                        <p
                                            className="text-sm font-semibold fg"
                                        >
                                            {weapon.criticalRange}+
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs fg-muted"
                                        >
                                            Multiplier
                                        </span>
                                        <p
                                            className="text-sm font-semibold fg"
                                        >
                                            ×{weapon.criticalMultiplier}
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs fg-muted"
                                        >
                                            Fail
                                        </span>
                                        <p
                                            className="text-sm font-semibold fg"
                                        >
                                            {weapon.criticalFailRange
                                                ? `${weapon.criticalFailRange}-`
                                                : '—'}
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div
                                className="border-t pt-3 border-subtle"
                            >
                                <div className="grid grid-cols-2 gap-2">
                                    <div>
                                        <span
                                            className="text-xs uppercase tracking-wide fg-muted"
                                        >
                                            Energy Type
                                        </span>
                                        <p
                                            className="text-sm font-semibold fg"
                                        >
                                            {weapon.energyType}
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs uppercase tracking-wide fg-muted"
                                        >
                                            Ammo Type
                                        </span>
                                        <p
                                            className="text-sm font-semibold fg"
                                        >
                                            {weapon.ammoType}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </details>
                ))}
            </div>
        )}
    </div>
)}
            </div>
        </div>
    )
}

