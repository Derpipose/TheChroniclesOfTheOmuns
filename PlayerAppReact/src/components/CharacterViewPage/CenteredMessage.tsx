import type { ReactNode } from 'react'

/** Full-page loading or not-found message. */
export default function CenteredMessage({ children }: { children: ReactNode }) {
  return (
    <div className="page flex items-center justify-center">
      <p className="text-sm fg-muted">{children}</p>
    </div>
  )
}
