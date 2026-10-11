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
            className="page"
        >
            {/* Header */}
            <NavBar title="Edit Weapon" />

            <div className="max-w-md mx-auto px-6 py-8 flex flex-col gap-5">

                {/* Selection */}
                <div
                    className="rounded-2xl border p-6 flex flex-col gap-4 panel"
                >
                    <h2
                        className="card-title"
                    >
                        Select Weapon
                    </h2>

                    {/* Character */}
                    <label className="flex flex-col gap-2">
                        <span
                            className="label"
                        >
                            Character
                        </span>

                        {charactersLoading ? (
                            <p
                                className="text-sm fg-muted"
                            >
                                Loading characters...
                            </p>
                        ) : characters.length === 0 ? (
                            <p
                                className="text-sm fg-muted"
                            >
                                No characters found.
                            </p>
                        ) : (
                            <select
                                value={selectedCharacterId}
                                onChange={handleCharacterChange}
                                className="field-input w-full"
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
                            className="label"
                        >
                            Weapon
                        </span>

                        {weaponsLoading ? (
                            <p
                                className="text-sm fg-muted"
                            >
                                Loading weapons...
                            </p>
                        ) : weapons.length === 0 ? (
                            <p
                                className="text-sm fg-muted"
                            >
                                This character has no weapons.
                            </p>
                        ) : (
                            <select
                                value={selectedWeaponId}
                                onChange={handleWeaponChange}
                                className="field-input w-full"
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
                        className="rounded-2xl border p-6 flex flex-col gap-5 panel"
                    >
                        <div>
                            <p
                                className="label"
                            >
                                Weapon
                            </p>

                            <h2
                                className="text-lg font-semibold mt-1 fg"
                            >
                                {selectedWeapon.name}
                            </h2>
                        </div>

                        {/* Nickname */}
                        <label className="flex flex-col gap-2">
                            <span
                                className="label"
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
                                className="field-input w-full"
                            />

                            <span
                                className="text-xs fg-muted"
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
                            className={`rounded-lg border border-subtle px-4 py-3 flex items-center justify-between transition-opacity hover:opacity-90 ${favorite ? 'bg-soft' : 'bg-page'}`}
                        >
                            <span
                                className="text-sm font-medium fg"
                            >
                                Favorite
                            </span>

                            <span
                                className={`text-lg ${favorite ? 'fg-accent' : 'fg-muted'}`}
                            >
                                {favorite ? '★' : '☆'}
                            </span>
                        </button>

                        {/* Weapon information */}
                        <div
                            className="rounded-lg px-3 py-3 flex flex-col gap-1 bg-soft"
                        >
                            <p
                                className="text-xs fg-muted"
                            >
                                Ammo
                            </p>

                            <p
                                className="text-sm font-semibold fg"
                            >
                                {selectedWeapon.currentAmmo}/
                                {selectedWeapon.ammoCapacity}
                            </p>
                        </div>

                        {error && (
                            <div
                                className="alert-error"
                            >
                                {error}
                            </div>
                        )}

                        {success && (
                            <div
                                className="alert-success"
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
                            className="btn btn-accent w-full"
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
                            className="btn btn-danger w-full border"
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
