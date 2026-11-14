import { useId, useState } from 'react'
import type { ReactNode } from 'react'
import { ChevronDown } from 'lucide-react'

interface PanelCardProps {
  title?: string
  /** Any icon element, e.g. <Heart /> from lucide-react. Sized and colored by the card. */
  icon?: ReactNode
  /** Right side of the header, e.g. a button. Clicks here do not toggle collapse. */
  action?: ReactNode
  /** Adds a chevron and lets the header collapse the body. */
  collapsible?: boolean
  defaultCollapsed?: boolean
  className?: string
  /** Extra classes for the content area, e.g. 'flex flex-col gap-4'. */
  bodyClassName?: string
  children: ReactNode
}

/** Parchment-style section wrapper: icon + serif title + divider, then any content. */
export default function PanelCard({
  title,
  icon,
  action,
  collapsible = false,
  defaultCollapsed = false,
  className = '',
  bodyClassName = '',
  children,
}: PanelCardProps) {
  const [collapsed, setCollapsed] = useState(collapsible && defaultCollapsed)
  const bodyId = useId()
  const hasHeader = title || icon || action

  const heading = (
    <>
      {icon && <span className="panel-card-icon">{icon}</span>}
      {title && <h3 className="panel-card-title">{title}</h3>}
    </>
  )

  return (
    <section className={`panel-card ${className}`}>
      {hasHeader && (
        <header className="panel-card-header">
          {collapsible ? (
            <button
              type="button"
              className="panel-card-toggle"
              aria-expanded={!collapsed}
              aria-controls={bodyId}
              onClick={() => setCollapsed((c) => !c)}
            >
              {heading}
              <ChevronDown
                size={18}
                className={`panel-card-chevron ${collapsed ? '-rotate-90' : ''}`}
              />
            </button>
          ) : (
            <div className="panel-card-toggle">{heading}</div>
          )}
          {action && <div className="panel-card-action">{action}</div>}
        </header>
      )}
      {!collapsed && (
        <div id={bodyId} className={`panel-card-body ${bodyClassName}`}>
          {children}
        </div>
      )}
    </section>
  )
}
