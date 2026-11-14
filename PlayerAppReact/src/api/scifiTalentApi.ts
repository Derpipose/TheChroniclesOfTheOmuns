import { apiFetch } from '../auth/apiFetch';
import type { ScifiTalent, ScifiTalentPick, ScifiTalentPreview } from '../types/scifiTalent';

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237';

export async function getScifiTalents(): Promise<ScifiTalent[]> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/scifiTalent`);
  if (!res.ok) throw new Error('Failed to fetch scifi talents');
  return res.json();
}

export async function getScifiTalentById(id: number): Promise<ScifiTalent> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/scifiTalent/${id}`);
  if (!res.ok) throw new Error('Failed to fetch scifi talent');
  return res.json();
}

export async function getTalentPicksByCharacterId(characterId: number): Promise<ScifiTalentPick[]> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/scifiTalent/TalentPicks/${characterId}`);
  if (!res.ok) throw new Error('Failed to fetch talent picks for character');
  return res.json();
}

export async function selectTalentForCharacter(characterId: number, talentId: number): Promise<void> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/scifiTalent/TalentPicks/${characterId}/${talentId}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
  });
  if (!res.ok) throw new Error('Failed to select talent for character');
}

export async function previewScifiTalentPicks(classId: number, backgroundId: number): Promise<ScifiTalentPreview[]> {
  const res = await apiFetch(`${BASE_URL}/api/scifi/scifiTalent/PreviewTalentPool/${classId}/${backgroundId}/`);
  if (!res.ok) throw new Error('Failed to fetch preview talent picks for character');
  return res.json();
}