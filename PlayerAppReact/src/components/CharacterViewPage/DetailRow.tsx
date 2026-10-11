/** "Race: Human" line; falls back to an em dash when there is no value. */
export default function DetailRow({ label, value }: { label: string; value?: string | null }) {
  return <p><span className="fg-muted">{label}:</span> {value ?? '—'}</p>
}
