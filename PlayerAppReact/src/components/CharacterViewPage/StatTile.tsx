import type { ReactNode } from 'react'

interface StatTileProps {
  label: string
  value: ReactNode
  py?: string
  labelSize?: string
  valueSize?: string
}

/** Boxed label-and-value tile. Size props are Tailwind classes. */
export default function StatTile({
  label,
  value,
  py = 'py-3',
  labelSize = 'text-xs',
  valueSize = 'text-lg',
}: StatTileProps) {
  return (
    <div className={`stat-tile ${py}`}>
      <span className={`stat-tile-label ${labelSize}`}>{label}</span>
      <span className={`stat-tile-value ${valueSize}`}>{value}</span>
    </div>
  )
}
