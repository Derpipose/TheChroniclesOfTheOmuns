import type { ReactNode } from 'react'

/** Label wrapping a form control. */
export default function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className="flex flex-col gap-1">
      <label className="text-xs font-medium fg-muted">{label}</label>
      {children}
    </div>
  )
}
