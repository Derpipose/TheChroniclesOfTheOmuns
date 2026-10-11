import type { ReactNode } from 'react'

type PillVariant = 'neutral' | 'accent' | 'active'

const VARIANT_CLASS: Record<PillVariant, string> = {
  neutral: '',
  accent: 'pill-accent',
  active: 'pill-active',
}

/** Small fully rounded label for a status or short fact ("Level 3", "Active"). */
export default function Pill({ variant = 'neutral', children }: { variant?: PillVariant; children: ReactNode }) {
  return <span className={`pill ${VARIANT_CLASS[variant]}`}>{children}</span>
}
