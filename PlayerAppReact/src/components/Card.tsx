import type { ReactNode } from 'react'

interface CardProps {
  title?: string
  description?: string
  className?: string
  children: ReactNode
}

/** Bordered panel with an optional title and description. Add layout utilities through className. */
export default function Card({ title, description, className = '', children }: CardProps) {
  const heading = title && (
    <h2 className="card-title">{title}</h2>
  )

  return (
    <div className={`card ${className}`}>
      {description ? (
        <div>
          {heading}
          <p className="text-xs mt-1 fg-muted">{description}</p>
        </div>
      ) : heading}
      {children}
    </div>
  )
}
