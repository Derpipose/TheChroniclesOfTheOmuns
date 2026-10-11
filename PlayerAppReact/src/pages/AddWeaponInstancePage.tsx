
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
                className="min-h-screen flex items-center justify-center"
                style={{ backgroundColor: 'var(--color-background)' }}
            >
                <p
                    className="text-sm"
                    style={{ color: 'var(--color-text-60)' }}
                >
                    Loading...
                </p>
            </div>
        )
    }

    return (
        <div
            className="min-h-screen"
            style={{ backgroundColor: 'var(--color-background)' }}
        >
                <NavBar title="Edit Weapon" />


            <div className="max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

                {/* Add weapon card */}
                <div
                    className="rounded-2xl border p-6 flex flex-col gap-4"
                    style={{
                        backgroundColor: 'var(--color-background-30)',
                        borderColor: 'var(--color-primary-30)',
                    }}
                >
                    <h2
                        className="text-sm font-semibold uppercase tracking-wide"
                        style={{ color: 'var(--color-text-60)' }}
                    >
                        Add Weapon
                    </h2>

                    {/* Character selector */}
                    <div className="flex flex-col gap-2">
                        <label
                            htmlFor="character"
                            className="text-xs font-medium uppercase tracking-wide"
                            style={{ color: 'var(--color-text-60)' }}
                        >
                            Character
                        </label>

                        <select
                            id="character"
                            value={selectedCharacterId}
                            onChange={(e) => setSelectedCharacterId(Number(e.target.value))}
                            className="rounded-lg border px-3 py-2 text-sm outline-none"
                            style={{
                                backgroundColor: 'var(--color-background)',
                                borderColor: 'var(--color-primary-30)',
                                color: 'var(--color-text)',
                            }}
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
                            className="text-xs font-medium uppercase tracking-wide"
                            style={{ color: 'var(--color-text-60)' }}
                        >
                            Weapon
                        </label>

                        <div className="relative">
    <button
        type="button"
        onClick={() => setWeaponDropdownOpen((open) => !open)}
        className="w-full rounded-lg border px-3 py-2 text-sm text-left outline-none"
        style={{
            backgroundColor: 'var(--color-background)',
            borderColor: 'var(--color-primary-30)',
            color: selectedWeaponId ? 'var(--color-text)' : 'var(--color-text-60)',
        }}
    >
        {selectedWeaponId
            ? weapons?.find((weapon) => weapon.id === selectedWeaponId)?.name
            : 'Select a weapon...'}
    </button>

    {weaponDropdownOpen && (
        <div
            className="absolute z-10 mt-1 w-full rounded-lg border p-2 shadow-sm"
            style={{
                backgroundColor: 'var(--color-background-30)',
                borderColor: 'var(--color-primary-30)',
            }}
        >
            <input
                type="text"
                value={weaponSearch}
                onChange={(e) => setWeaponSearch(e.target.value)}
                placeholder="Search weapons..."
                autoFocus
                className="w-full rounded-lg border px-3 py-2 text-sm outline-none"
                style={{
                    backgroundColor: 'var(--color-background)',
                    borderColor: 'var(--color-primary-30)',
                    color: 'var(--color-text)',
                }}
            />

            <div className="mt-2 max-h-60 overflow-y-auto">
                {filteredWeapons.length === 0 ? (
                    <p
                        className="px-3 py-2 text-sm"
                        style={{ color: 'var(--color-text-60)' }}
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
                            className="w-full rounded-lg px-3 py-2 text-left text-sm transition-colors hover:opacity-80"
                            style={{
                                backgroundColor:
                                    selectedWeaponId === weapon.id
                                        ? 'var(--color-background-60)'
                                        : 'transparent',
                                color: 'var(--color-text)',
                            }}
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
                            className="rounded-lg p-3 flex flex-col gap-1"
                            style={{ backgroundColor: 'var(--color-background-60)' }}
                        >
                            <span
                                className="text-xs font-medium uppercase tracking-wide"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Selected Weapon
                            </span>

                            <span
                                className="text-sm font-semibold"
                                style={{ color: 'var(--color-text)' }}
                            >
                                {selectedWeapon.name}
                            </span>

                            <span
                                className="text-xs"
                                style={{ color: 'var(--color-text-60)' }}
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
                        className="btn"
                        style={{
                            backgroundColor: 'var(--color-accent)',
                            color: 'var(--color-text-30)',
                        }}
                    >
                        {createWeaponInstance.isPending
                            ? 'Adding...'
                            : 'Add Weapon'}
                    </button>

                    {createWeaponInstance.isError && (
                        <p
                            className="text-xs"
                            style={{ color: 'var(--color-secondary)' }}
                        >
                            Failed to add weapon. Please try again.
                        </p>
                    )}

                    {createWeaponInstance.isSuccess && (
                        <p
                            className="text-xs"
                            style={{ color: '#3D5C3D' }}
                        >
                            {selectedWeapon?.name ?? 'Weapon'} added to{' '}
                            {selectedCharacter?.name ?? 'character'}.
                        </p>
                    )}
                </div>

                {/* Existing weapons */}
                {selectedCharacterId > 0 && (
    <div
        className="rounded-2xl border p-6 flex flex-col gap-3"
        style={{
            backgroundColor: 'var(--color-background-30)',
            borderColor: 'var(--color-primary-30)',
        }}
    >
        <div className="flex items-center justify-between">
            <h2
                className="text-sm font-semibold uppercase tracking-wide"
                style={{ color: 'var(--color-text-60)' }}
            >
                {selectedCharacter?.name ?? 'Character'}'s Weapons
            </h2>

            <span
                className="text-xs"
                style={{ color: 'var(--color-text-60)' }}
            >
                {existingWeapons?.length ?? 0}
            </span>
        </div>

        {existingWeaponsLoading ? (
            <p
                className="text-sm"
                style={{ color: 'var(--color-text-60)' }}
            >
                Loading weapons...
            </p>
        ) : existingWeapons?.length === 0 ? (
            <p
                className="text-sm"
                style={{ color: 'var(--color-text-60)' }}
            >
                This character has no weapons.
            </p>
        ) : (
            <div className="flex flex-col gap-2">
                {existingWeapons?.map((weapon) => (
                    <details
                        key={weapon.id}
                        className="rounded-lg overflow-hidden"
                        style={{ backgroundColor: 'var(--color-background-60)' }}
                    >
                        <summary
                            className="list-none [&::-webkit-details-marker]:hidden px-3 py-3 flex items-center justify-between cursor-pointer"
                            style={{ color: 'var(--color-text)' }}
                        >
                            <div className="flex items-center gap-2">

                                <div className="flex flex-col">
                                    <span className="text-sm font-semibold">
                                        {weapon.nickname || weapon.name}
                                    </span>

                                    {weapon.nickname &&
                                        weapon.nickname !== weapon.name && (
                                            <span
                                                className="text-xs"
                                                style={{ color: 'var(--color-text-60)' }}
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
                                    className="text-xs"
                                    style={{ color: 'var(--color-text-60)' }}
                                >
                                    {weapon.currentAmmo}/{weapon.ammoCapacity}
                                </span>
                            </div>
                        </summary>

                        <div
                            className="border-t px-3 py-3 flex flex-col gap-3"
                            style={{ borderColor: 'var(--color-primary-30)' }}
                        >
                            <div className="grid grid-cols-2 gap-2">
                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: 'var(--color-text-60)' }}
                                    >
                                        Damage
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: 'var(--color-text)' }}
                                    >
                                        {weapon.damageFormula}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: 'var(--color-text-60)' }}
                                    >
                                        Damage Type
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: 'var(--color-text)' }}
                                    >
                                        {weapon.damageType}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: 'var(--color-text-60)' }}
                                    >
                                        Modifier
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: 'var(--color-text)' }}
                                    >
                                        {weapon.modifierStat}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: 'var(--color-text-60)' }}
                                    >
                                        Range
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: 'var(--color-text)' }}
                                    >
                                        {weapon.rangeCategory}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: 'var(--color-text-60)' }}
                                    >
                                        Rate of Fire
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: 'var(--color-text)' }}
                                    >
                                        {weapon.rateOfFire}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: 'var(--color-text-60)' }}
                                    >
                                        Reload
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: 'var(--color-text)' }}
                                    >
                                        {weapon.reloadTimeInActions} actions
                                    </p>
                                </div>
                            </div>

                            <div
                                className="border-t pt-3"
                                style={{ borderColor: 'var(--color-primary-30)' }}
                            >
                                <span
                                    className="text-xs font-medium uppercase tracking-wide"
                                    style={{ color: 'var(--color-text-60)' }}
                                >
                                    Critical
                                </span>

                                <div className="grid grid-cols-3 gap-2 mt-2">
                                    <div>
                                        <span
                                            className="text-xs"
                                            style={{ color: 'var(--color-text-60)' }}
                                        >
                                            Range
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: 'var(--color-text)' }}
                                        >
                                            {weapon.criticalRange}+
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs"
                                            style={{ color: 'var(--color-text-60)' }}
                                        >
                                            Multiplier
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: 'var(--color-text)' }}
                                        >
                                            ×{weapon.criticalMultiplier}
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs"
                                            style={{ color: 'var(--color-text-60)' }}
                                        >
                                            Fail
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: 'var(--color-text)' }}
                                        >
                                            {weapon.criticalFailRange
                                                ? `${weapon.criticalFailRange}-`
                                                : '—'}
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div
                                className="border-t pt-3"
                                style={{ borderColor: 'var(--color-primary-30)' }}
                            >
                                <div className="grid grid-cols-2 gap-2">
                                    <div>
                                        <span
                                            className="text-xs uppercase tracking-wide"
                                            style={{ color: 'var(--color-text-60)' }}
                                        >
                                            Energy Type
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: 'var(--color-text)' }}
                                        >
                                            {weapon.energyType}
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs uppercase tracking-wide"
                                            style={{ color: 'var(--color-text-60)' }}
                                        >
                                            Ammo Type
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: 'var(--color-text)' }}
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

