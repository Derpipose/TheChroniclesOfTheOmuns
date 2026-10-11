import type { SeedStatus } from '../../types/devSeed'

const STATUS_CLASS: Record<SeedStatus, string> = {
  success: 'fg-success',
  error: 'fg-danger',
  loading: 'fg-accent',
  idle: 'fg-muted',
}

const STATUS_LABEL: Record<SeedStatus, string> = {
  success: '✓ Done',
  error: '✗ Failed',
  loading: 'Loading...',
  idle: 'Not run',
}

interface SeedButtonProps {
  label: string
  status: SeedStatus
  message?: string
  onClick: () => void
}

/** One row of the seeding list: label, last run status, and the Seed action. */
export default function SeedButton({ label, status, message, onClick }: SeedButtonProps) {
  return (
    <div className="rounded-xl border px-5 py-4 flex flex-col gap-1 panel">
      <div className="flex items-center justify-between">
        <div>
          <p className="font-semibold fg">{label}</p>
          <p className={`text-xs mt-0.5 ${STATUS_CLASS[status]}`}>{STATUS_LABEL[status]}</p>
        </div>
        <button
          onClick={onClick}
          disabled={status === 'loading'}
          className="btn px-4 btn-accent"
        >
          Seed
        </button>
      </div>
      {message && <p className="text-xs break-words fg-muted">{message}</p>}
    </div>
  )
}
