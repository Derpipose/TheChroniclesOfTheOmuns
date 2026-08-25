
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCharacters } from '../hooks/useCharacters'
import { useScifiWeapons } from '../hooks/useScifiWeapon'
import {
    useCreateScifiWeaponInstance,
    useScifiWeaponInstances,
} from '../hooks/useScifiWeaponInstance'

export default function AddWeaponInstancePage() {
    const navigate = useNavigate()

    const [selectedCharacterId, setSelectedCharacterId] = useState<number>(0)
    const [selectedWeaponId, setSelectedWeaponId] = useState<number>(0)

    const { data: characters, isLoading: charactersLoading } = useCharacters()
    const { data: weapons, isLoading: weaponsLoading } = useScifiWeapons()

    const {
        data: existingWeapons,
        isLoading: existingWeaponsLoading,
    } = useScifiWeaponInstances(selectedCharacterId)

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

    createWeaponInstance.mutate({
        scifiWeaponId: selectedWeaponId,
        scifiCharacterId: selectedCharacterId,
        scifiWeaponBlueprintId: selectedWeaponId,
    })
}

    if (charactersLoading || weaponsLoading) {
        return (
            <div
                className="min-h-screen flex items-center justify-center"
                style={{ backgroundColor: '#F5F0E8' }}
            >
                <p
                    className="text-sm"
                    style={{ color: '#7A6F5C' }}
                >
                    Loading...
                </p>
            </div>
        )
    }

    return (
        <div
            className="min-h-screen"
            style={{ backgroundColor: '#F5F0E8' }}
        >
            <div
                className="border-b px-6 py-4 flex items-center gap-3"
                style={{
                    backgroundColor: '#FDFAF4',
                    borderColor: '#DDD8CC',
                }}
            >
                <button
                    onClick={() => navigate(-1)}
                    className="text-sm transition-colors hover:underline"
                    style={{ color: '#7A6F5C' }}
                >
                    ← Back
                </button>

                <h1
                    className="text-lg font-semibold"
                    style={{ color: '#2C2416' }}
                >
                    Add Weapon
                </h1>
            </div>

            <div className="max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

                {/* Add weapon card */}
                <div
                    className="rounded-2xl border p-6 flex flex-col gap-4"
                    style={{
                        backgroundColor: '#FDFAF4',
                        borderColor: '#DDD8CC',
                    }}
                >
                    <h2
                        className="text-sm font-semibold uppercase tracking-wide"
                        style={{ color: '#7A6F5C' }}
                    >
                        Add Weapon
                    </h2>

                    {/* Character selector */}
                    <div className="flex flex-col gap-2">
                        <label
                            htmlFor="character"
                            className="text-xs font-medium uppercase tracking-wide"
                            style={{ color: '#7A6F5C' }}
                        >
                            Character
                        </label>

                        <select
                            id="character"
                            value={selectedCharacterId}
                            onChange={(e) => setSelectedCharacterId(Number(e.target.value))}
                            className="rounded-lg border px-3 py-2 text-sm outline-none"
                            style={{
                                backgroundColor: '#F5F0E8',
                                borderColor: '#DDD8CC',
                                color: '#2C2416',
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
                            style={{ color: '#7A6F5C' }}
                        >
                            Weapon
                        </label>

                        <select
                            id="weapon"
                            value={selectedWeaponId}
                            onChange={(e) =>
                                setSelectedWeaponId(Number(e.target.value))
                            }
                            className="rounded-lg border px-3 py-2 text-sm outline-none"
                            style={{
                                backgroundColor: '#F5F0E8',
                                borderColor: '#DDD8CC',
                                color: '#2C2416',
                            }}
                        >
                            <option value={0}>
                                Select a weapon...
                            </option>

                            {weapons?.map((weapon) => (
                                <option
                                    key={weapon.id}
                                    value={weapon.id}
                                >
                                    {weapon.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    {/* Selected weapon preview */}
                    {selectedWeapon && (
                        <div
                            className="rounded-lg p-3 flex flex-col gap-1"
                            style={{ backgroundColor: '#EDE8DC' }}
                        >
                            <span
                                className="text-xs font-medium uppercase tracking-wide"
                                style={{ color: '#7A6F5C' }}
                            >
                                Selected Weapon
                            </span>

                            <span
                                className="text-sm font-semibold"
                                style={{ color: '#2C2416' }}
                            >
                                {selectedWeapon.name}
                            </span>

                            <span
                                className="text-xs"
                                style={{ color: '#7A6F5C' }}
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
                            createWeaponInstance.isPending
                        }
                        className="rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50"
                        style={{
                            backgroundColor: '#8B7355',
                            color: '#FDFAF4',
                        }}
                    >
                        {createWeaponInstance.isPending
                            ? 'Adding...'
                            : 'Add Weapon'}
                    </button>

                    {createWeaponInstance.isError && (
                        <p
                            className="text-xs"
                            style={{ color: '#8B4A3A' }}
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
            backgroundColor: '#FDFAF4',
            borderColor: '#DDD8CC',
        }}
    >
        <div className="flex items-center justify-between">
            <h2
                className="text-sm font-semibold uppercase tracking-wide"
                style={{ color: '#7A6F5C' }}
            >
                {selectedCharacter?.name ?? 'Character'}'s Weapons
            </h2>

            <span
                className="text-xs"
                style={{ color: '#7A6F5C' }}
            >
                {existingWeapons?.length ?? 0}
            </span>
        </div>

        {existingWeaponsLoading ? (
            <p
                className="text-sm"
                style={{ color: '#7A6F5C' }}
            >
                Loading weapons...
            </p>
        ) : existingWeapons?.length === 0 ? (
            <p
                className="text-sm"
                style={{ color: '#7A6F5C' }}
            >
                This character has no weapons.
            </p>
        ) : (
            <div className="flex flex-col gap-2">
                {existingWeapons?.map((weapon) => (
                    <details
                        key={weapon.id}
                        className="rounded-lg overflow-hidden"
                        style={{ backgroundColor: '#EDE8DC' }}
                    >
                        <summary
                            className="list-none [&::-webkit-details-marker]:hidden px-3 py-3 flex items-center justify-between cursor-pointer"
                            style={{ color: '#2C2416' }}
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
                                                style={{ color: '#7A6F5C' }}
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
                                    style={{ color: '#7A6F5C' }}
                                >
                                    {weapon.currentAmmo}/{weapon.ammoCapacity}
                                </span>
                            </div>
                        </summary>

                        <div
                            className="border-t px-3 py-3 flex flex-col gap-3"
                            style={{ borderColor: '#DDD8CC' }}
                        >
                            <div className="grid grid-cols-2 gap-2">
                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: '#7A6F5C' }}
                                    >
                                        Damage
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: '#2C2416' }}
                                    >
                                        {weapon.damageFormula}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: '#7A6F5C' }}
                                    >
                                        Damage Type
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: '#2C2416' }}
                                    >
                                        {weapon.damageType}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: '#7A6F5C' }}
                                    >
                                        Modifier
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: '#2C2416' }}
                                    >
                                        {weapon.modifierStat}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: '#7A6F5C' }}
                                    >
                                        Range
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: '#2C2416' }}
                                    >
                                        {weapon.rangeCategory}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: '#7A6F5C' }}
                                    >
                                        Rate of Fire
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: '#2C2416' }}
                                    >
                                        {weapon.rateOfFire}
                                    </p>
                                </div>

                                <div>
                                    <span
                                        className="text-xs uppercase tracking-wide"
                                        style={{ color: '#7A6F5C' }}
                                    >
                                        Reload
                                    </span>
                                    <p
                                        className="text-sm font-semibold"
                                        style={{ color: '#2C2416' }}
                                    >
                                        {weapon.reloadTimeInActions} actions
                                    </p>
                                </div>
                            </div>

                            <div
                                className="border-t pt-3"
                                style={{ borderColor: '#DDD8CC' }}
                            >
                                <span
                                    className="text-xs font-medium uppercase tracking-wide"
                                    style={{ color: '#7A6F5C' }}
                                >
                                    Critical
                                </span>

                                <div className="grid grid-cols-3 gap-2 mt-2">
                                    <div>
                                        <span
                                            className="text-xs"
                                            style={{ color: '#7A6F5C' }}
                                        >
                                            Range
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: '#2C2416' }}
                                        >
                                            {weapon.criticalRange}+
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs"
                                            style={{ color: '#7A6F5C' }}
                                        >
                                            Multiplier
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: '#2C2416' }}
                                        >
                                            ×{weapon.criticalMultiplier}
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs"
                                            style={{ color: '#7A6F5C' }}
                                        >
                                            Fail
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: '#2C2416' }}
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
                                style={{ borderColor: '#DDD8CC' }}
                            >
                                <div className="grid grid-cols-2 gap-2">
                                    <div>
                                        <span
                                            className="text-xs uppercase tracking-wide"
                                            style={{ color: '#7A6F5C' }}
                                        >
                                            Energy Type
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: '#2C2416' }}
                                        >
                                            {weapon.energyType}
                                        </p>
                                    </div>

                                    <div>
                                        <span
                                            className="text-xs uppercase tracking-wide"
                                            style={{ color: '#7A6F5C' }}
                                        >
                                            Ammo Type
                                        </span>
                                        <p
                                            className="text-sm font-semibold"
                                            style={{ color: '#2C2416' }}
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

