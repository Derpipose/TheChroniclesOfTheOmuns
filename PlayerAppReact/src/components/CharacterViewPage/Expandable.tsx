/** A name that expands to show its description, or plain text when there is none. */
export default function Expandable({ name, description }: { name: string; description?: string | null }) {
  if (!description) return <p className="text-sm font-semibold fg">{name}</p>

  return (
    <details className="group">
      <summary className="collapse-summary">
        <span className="fg-muted transition-transform group-open:rotate-90">›</span>
        {name}
      </summary>
      <p className="text-sm mt-1 pl-4 fg-muted">{description}</p>
    </details>
  )
}
