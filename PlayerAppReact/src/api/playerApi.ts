import {apiFetch} from '../auth/apiFetch';
import type {Player} from '../types/player'


const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237';

export async function getPlayer(): Promise<Player> {
    const response = await apiFetch(`${BASE_URL}/api/player/details`);
    if (!response.ok) {
        throw new Error(`Failed to fetch player data: ${response.statusText}`);
    }
    return response.json();
}

export async function setPlayerTheme(theme: string): Promise<void> {
    const response = await apiFetch(`${BASE_URL}/api/player/theme`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ theme }),
    });

    if (!response.ok) {
        throw new Error(`Failed to set player theme: ${response.statusText}`);
    }
}