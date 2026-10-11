import Field from './Field'

/** Select generated from a const-object enum such as DamageType. */
export default function EnumSelect<T extends string>({ label, value, options, onChange }: {
  label: string
  value: T
  options: Record<string, T>
  onChange: (value: T) => void
}) {
  return (
    <Field label={label}>
      <select
        value={value}
        onChange={(event) => onChange(event.target.value as T)}
        className="field-input"
      >
        {Object.values(options).map((option) => (
          <option key={option} value={option}>{option}</option>
        ))}
      </select>
    </Field>
  )
}
