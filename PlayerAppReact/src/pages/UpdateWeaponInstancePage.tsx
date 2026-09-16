import { useEffect, useState } from 'react'
import { useCharacters, useScifiCharacterByCharacterId } from '../hooks/useCharacters'
import {
    useScifiWeaponInstances,
    useScifiWeaponInstance,
    useDeleteScifiWeaponInstance,
    useRenameWeapon,
    useToggleFavorite,
} from '../hooks/useScifiWeaponInstance'

import NavBar from '../components/NavBar'

export default function UpdateWeaponInstancePage() {
    const { data: characters = [], isLoading: charactersLoading } = useCharacters()

    const [selectedCharacterId, setSelectedCharacterId] = useState<number | ''>('')
    const [selectedWeaponId, setSelectedWeaponId] = useState<number | ''>('')
    const { data: selectedScifiCharacter } = useScifiCharacterByCharacterId(selectedCharacterId || 0)

    const {
        data: weapons = [],
        isLoading: weaponsLoading,
    } = useScifiWeaponInstances(selectedScifiCharacter?.id || 0)

    const { data: selectedWeapon } = useScifiWeaponInstance(
        selectedWeaponId || 0
    )

    const renameMutation = useRenameWeapon()
    const favoriteMutation = useToggleFavorite()
    const deleteMutation = useDeleteScifiWeaponInstance()

    const [nickname, setNickname] = useState('')
    const [favorite, setFavorite] = useState(false)

    const [saving, setSaving] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

    // Select the first character when characters load.
    useEffect(() => {
        if (characters.length > 0 && !selectedCharacterId) {
            setSelectedCharacterId(characters[0].id)
        }
    }, [characters, selectedCharacterId])

    // Select the first weapon when the character changes.
    useEffect(() => {
        if (weapons.length > 0) {
            setSelectedWeaponId(weapons[0].id)
        } else {
            setSelectedWeaponId('')
        }
    }, [weapons])

    // Populate the editor whenever the selected weapon changes.
    useEffect(() => {
        if (selectedWeapon) {
            setNickname(selectedWeapon.nickname)
            setFavorite(selectedWeapon.favorite)
            setError(null)
            setSuccess(false)
        }
    }, [selectedWeapon])

    const handleCharacterChange = (
        event: React.ChangeEvent<HTMLSelectElement>
    ) => {
        const id = Number(event.target.value)

        setSelectedCharacterId(id)
        setSelectedWeaponId('')
        setError(null)
        setSuccess(false)
    }

    const handleWeaponChange = (
        event: React.ChangeEvent<HTMLSelectElement>
    ) => {
        const id = Number(event.target.value)

        setSelectedWeaponId(id)
        setError(null)
        setSuccess(false)
    }

    const handleSave = async () => {
        if (!selectedWeapon) return

        try {
            setSaving(true)
            setError(null)
            setSuccess(false)

            let updatedWeapon = selectedWeapon

            // Update nickname if it changed.
            if (nickname !== selectedWeapon.nickname) {
                updatedWeapon = await renameMutation.mutateAsync({
                    id: selectedWeapon.id,
                    newName: nickname,
                })
            }

            // Toggle favorite if the desired state differs.
            if (favorite !== updatedWeapon.favorite) {
                updatedWeapon = await favoriteMutation.mutateAsync(
                    updatedWeapon.id
                )
            }

            setNickname(updatedWeapon.nickname)
            setFavorite(updatedWeapon.favorite)
            setSuccess(true)
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Failed to update weapon.'
            )
        } finally {
            setSaving(false)
        }
    }

    const handleDelete = async () => {
        if (!selectedWeapon) return

        const weaponName = selectedWeapon.nickname || selectedWeapon.name
        if (!window.confirm(`Delete ${weaponName} from this character?`)) return

        try {
            setError(null)
            setSuccess(false)
            await deleteMutation.mutateAsync(selectedWeapon.id)
            setSelectedWeaponId('')
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Failed to delete weapon.'
            )
        }
    }

    return (
        <div
            className="min-h-screen"
            style={{ backgroundColor: 'var(--color-background)' }}
        >
            {/* Header */}
            <NavBar title="Edit Weapon" />

            <div className="max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

                {/* Selection */}
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
                        Select Weapon
                    </h2>

                    {/* Character */}
                    <label className="flex flex-col gap-2">
                        <span
                            className="text-xs font-medium uppercase tracking-wide"
                            style={{ color: 'var(--color-text-60)' }}
                        >
                            Character
                        </span>

                        {charactersLoading ? (
                            <p
                                className="text-sm"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Loading characters...
                            </p>
                        ) : characters.length === 0 ? (
                            <p
                                className="text-sm"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                No characters found.
                            </p>
                        ) : (
                            <select
                                value={selectedCharacterId}
                                onChange={handleCharacterChange}
                                className="w-full rounded-lg border px-3 py-2 text-sm outline-none"
                                style={{
                                    backgroundColor: 'var(--color-background)',
                                    borderColor: 'var(--color-primary-30)',
                                    color: 'var(--color-text)',
                                }}
                            >
                                {characters.map((character) => (
                                    <option
                                        key={character.id}
                                        value={character.id}
                                    >
                                        {character.name}
                                    </option>
                                ))}
                            </select>
                        )}
                    </label>

                    {/* Weapon */}
                    <label className="flex flex-col gap-2">
                        <span
                            className="text-xs font-medium uppercase tracking-wide"
                            style={{ color: 'var(--color-text-60)' }}
                        >
                            Weapon
                        </span>

                        {weaponsLoading ? (
                            <p
                                className="text-sm"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Loading weapons...
                            </p>
                        ) : weapons.length === 0 ? (
                            <p
                                className="text-sm"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                This character has no weapons.
                            </p>
                        ) : (
                            <select
                                value={selectedWeaponId}
                                onChange={handleWeaponChange}
                                className="w-full rounded-lg border px-3 py-2 text-sm outline-none"
                                style={{
                                    backgroundColor: 'var(--color-background)',
                                    borderColor: 'var(--color-primary-30)',
                                    color: 'var(--color-text)',
                                }}
                            >
                                {weapons.map((weapon) => (
                                    <option
                                        key={weapon.id}
                                        value={weapon.id}
                                    >
                                        {weapon.nickname || weapon.name}
                                    </option>
                                ))}
                            </select>
                        )}
                    </label>
                </div>

                {/* Weapon editor */}
                {selectedWeapon && (
                    <div
                        className="rounded-2xl border p-6 flex flex-col gap-5"
                        style={{
                            backgroundColor: 'var(--color-background-30)',
                            borderColor: 'var(--color-primary-30)',
                        }}
                    >
                        <div>
                            <p
                                className="text-xs font-medium uppercase tracking-wide"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Weapon
                            </p>

                            <h2
                                className="text-lg font-semibold mt-1"
                                style={{ color: 'var(--color-text)' }}
                            >
                                {selectedWeapon.name}
                            </h2>
                        </div>

                        {/* Nickname */}
                        <label className="flex flex-col gap-2">
                            <span
                                className="text-xs font-medium uppercase tracking-wide"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Nickname
                            </span>

                            <input
                                type="text"
                                value={nickname}
                                onChange={(event) =>
                                    setNickname(event.target.value)
                                }
                                placeholder={selectedWeapon.name}
                                className="w-full rounded-lg border px-3 py-2 text-sm outline-none"
                                style={{
                                    backgroundColor: 'var(--color-background)',
                                    borderColor: 'var(--color-primary-30)',
                                    color: 'var(--color-text)',
                                }}
                            />

                            <span
                                className="text-xs"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Leave blank to use the weapon's normal name.
                            </span>
                        </label>

                        {/* Favorite */}
                        <button
                            type="button"
                            onClick={() =>
                                setFavorite((current) => !current)
                            }
                            className="rounded-lg border px-4 py-3 flex items-center justify-between transition-opacity hover:opacity-90"
                            style={{
                                backgroundColor: favorite
                                    ? 'var(--color-background-60)'
                                    : 'var(--color-background)',
                                borderColor: 'var(--color-primary-30)',
                            }}
                        >
                            <span
                                className="text-sm font-medium"
                                style={{ color: 'var(--color-text)' }}
                            >
                                Favorite
                            </span>

                            <span
                                className="text-lg"
                                style={{
                                    color: favorite
                                        ? 'var(--color-accent)'
                                        : 'var(--color-text-60)',
                                }}
                            >
                                {favorite ? '★' : '☆'}
                            </span>
                        </button>

                        {/* Weapon information */}
                        <div
                            className="rounded-lg px-3 py-3 flex flex-col gap-1"
                            style={{ backgroundColor: 'var(--color-background-60)' }}
                        >
                            <p
                                className="text-xs"
                                style={{ color: 'var(--color-text-60)' }}
                            >
                                Ammo
                            </p>

                            <p
                                className="text-sm font-semibold"
                                style={{ color: 'var(--color-text)' }}
                            >
                                {selectedWeapon.currentAmmo}/
                                {selectedWeapon.ammoCapacity}
                            </p>
                        </div>

                        {error && (
                            <div
                                className="rounded-lg p-3 text-sm"
                                style={{
                                    backgroundColor: 'var(--color-background)',
                                    color: 'var(--color-secondary)',
                                    border: '1px solid var(--color-primary-30)',
                                }}
                            >
                                {error}
                            </div>
                        )}

                        {success && (
                            <div
                                className="rounded-lg p-3 text-sm"
                                style={{
                                    backgroundColor: '#E4EEE4',
                                    color: '#3D5C3D',
                                    border: '1px solid #C9D9C9',
                                }}
                            >
                                Weapon updated successfully.
                            </div>
                        )}

                        <button
                            type="button"
                            onClick={handleSave}
                            disabled={
                                saving ||
                                renameMutation.isPending ||
                                favoriteMutation.isPending ||
                                deleteMutation.isPending
                            }
                            className="w-full rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 disabled:opacity-50"
                            style={{
                                backgroundColor: 'var(--color-accent)',
                                color: 'var(--color-text-30)',
                            }}
                        >
                            {saving ? 'Saving...' : 'Save Changes'}
                        </button>

                        <button
                            type="button"
                            onClick={handleDelete}
                            disabled={
                                saving ||
                                renameMutation.isPending ||
                                favoriteMutation.isPending ||
                                deleteMutation.isPending
                            }
                            className="w-full rounded-lg border py-2 text-sm font-semibold transition-opacity hover:opacity-90 disabled:opacity-50"
                            style={{
                                backgroundColor: 'var(--color-secondary-30)',
                                borderColor: 'var(--color-secondary-60)',
                                color: 'var(--color-secondary)',
                            }}
                        >
                            {deleteMutation.isPending
                                ? 'Deleting...'
                                : 'Delete Weapon'}
                        </button>
                    </div>
                )}
            </div>
        </div>
    )
}
