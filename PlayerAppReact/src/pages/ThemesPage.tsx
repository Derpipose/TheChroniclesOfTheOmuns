import { useEffect, useState } from 'react'
import Card from '../components/Card'
import Pill from '../components/Pill'
import StatTile from '../components/CharacterViewPage/StatTile'
import Expandable from '../components/CharacterViewPage/Expandable'
import DetailRow from '../components/CharacterViewPage/DetailRow'
import { AVAILABLE_THEMES } from '../theme/availableThemes'
import { getTheme } from '../theme/themeStorage'

const SWATCH_GROUPS = ['primary', 'secondary', 'text', 'background', 'accent'] as const
const SWATCH_STEPS = ['', '-60', '-30'] as const

/** Theme preview sandbox at /themes. Switching here only previews; nothing is saved. */
export default function ThemesPage() {
  const [theme, setTheme] = useState(
    () => document.documentElement.getAttribute('data-theme') ?? getTheme(),
  )
  const [tab, setTab] = useState<'one' | 'two'>('one')
  const [mode, setMode] = useState('normal')
  const [selected, setSelected] = useState('General')
  const [showToast, setShowToast] = useState(true)

  useEffect(() => {
    const previous = document.documentElement.getAttribute('data-theme')
    document.documentElement.setAttribute('data-theme', theme)
    return () => {
      if (previous) document.documentElement.setAttribute('data-theme', previous)
    }
  }, [theme])

  return (
    <div className="page">
      {/* Header bar + nav buttons */}
      <div className="app-header">
        <h1 className="text-lg font-semibold shrink-0 fg">Theme Playground</h1>
        <div className="flex flex-1 flex-wrap items-center gap-1.5 sm:gap-2">
          <button className="nav-btn">Dice Roller</button>
          <button className="nav-btn">Add Weapon</button>
          <button className="nav-btn">Settings</button>
        </div>
        <div className="ml-auto shrink-0">
          <button className="nav-btn">Sign out</button>
        </div>
      </div>

      {showToast && (
        <div className="toast inset-x-4 top-24 sm:inset-x-auto sm:right-6 sm:w-auto sm:min-w-72">
          <span className="text-sm font-semibold">Toast: you rolled a 20!</span>
          <button onClick={() => setShowToast(false)} className="px-1 text-lg leading-none fg-muted">×</button>
        </div>
      )}

      <div className="max-w-5xl mx-auto px-4 py-8 sm:px-6 flex flex-col gap-5">
        <h2 className="page-title">Themes</h2>

        {/* Theme picker */}
        <Card title="Theme" description="Preview only. Does not save to your profile.">
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 mt-3">
            {AVAILABLE_THEMES.map((t) => (
              <button
                key={t.value}
                onClick={() => setTheme(t.value)}
                className={`choice-btn ${t.value === theme ? 'choice-btn-active' : ''}`}
              >
                <span className="flex items-center justify-between">
                  {t.label}
                  {t.value === theme && <span className="text-xs">Active</span>}
                </span>
              </button>
            ))}
          </div>
        </Card>

        {/* Raw palette */}
        <Card title="Palette" description="Every CSS variable in the active theme.">
          <div className="flex flex-col gap-2 mt-3">
            {SWATCH_GROUPS.map((group) => (
              <div key={group} className="grid grid-cols-3 gap-2">
                {SWATCH_STEPS.map((step) => {
                  const name = `--color-${group}${step}`
                  return (
                    <div key={name} className="flex flex-col gap-1">
                      <div
                        className="h-10 rounded-lg border"
                        style={{ backgroundColor: `var(${name})`, borderColor: 'var(--color-primary-60)' }}
                      />
                      <span className="text-[10px] fg-muted">{name}</span>
                    </div>
                  )
                })}
              </div>
            ))}
          </div>
        </Card>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
          {/* Typography */}
          <Card title="Typography">
            <div className="flex flex-col gap-2 mt-3">
              <p className="page-title">Page title</p>
              <p className="text-sm fg">Body text in the primary text color.</p>
              <p className="text-sm fg-muted">Muted text for descriptions and hints.</p>
              <p className="text-sm fg-accent">Accent text for emphasis.</p>
              <p className="text-sm fg-danger">Danger text for errors.</p>
              <p className="text-sm fg-success">Success text for confirmations.</p>
              <p className="label">Label text</p>
              <DetailRow label="Race" value="Human" />
              <DetailRow label="Class" />
            </div>
          </Card>

          {/* Pills + status */}
          <Card title="Pills & status">
            <div className="flex flex-col gap-3 mt-3">
              <div className="flex flex-wrap gap-2">
                <Pill>Neutral</Pill>
                <Pill variant="accent">Accent</Pill>
                <Pill variant="active">Active</Pill>
              </div>
              <div className="flex items-center gap-2 text-sm fg">
                <span className="status-dot" /> Idle
                <span className="status-dot status-dot-active ml-4" /> Online
              </div>
              <div className="progress"><div className="progress-fill" style={{ width: '65%' }} /></div>
              <div className="flex items-center gap-3">
                <div className="avatar">O</div>
                <div className="flex flex-col gap-2 flex-1">
                  <div className="row-chip"><span className="fg-muted">HP</span><span className="fg">24 / 30</span></div>
                  <div className="row-chip"><span className="fg-muted">AC</span><span className="fg">15</span></div>
                </div>
              </div>
            </div>
          </Card>

          {/* Buttons */}
          <Card title="Buttons">
            <div className="flex flex-col gap-3 mt-3">
              <div className="flex flex-wrap gap-2">
                <button className="btn btn-accent px-4">Accent</button>
                <button className="btn btn-neutral px-4">Neutral</button>
                <button className="btn btn-danger border px-4">Danger</button>
                <button className="btn btn-alert border px-4">Alert</button>
                <button className="btn btn-accent px-4" disabled>Disabled</button>
              </div>
              <div className="flex flex-wrap gap-2">
                <button className="btn-sm btn-accent">Small accent</button>
                <button className="btn-sm btn-neutral">Small neutral</button>
              </div>
              <div className="flex gap-2">
                <button className={`tab-btn ${tab === 'one' ? 'tab-btn-active' : ''}`} onClick={() => setTab('one')}>Tab one</button>
                <button className={`tab-btn ${tab === 'two' ? 'tab-btn-active' : ''}`} onClick={() => setTab('two')}>Tab two</button>
              </div>
              <div className="flex flex-wrap gap-2">
                {['normal', 'advantage', 'disadvantage'].map((m) => (
                  <label key={m} className={`mode-chip ${mode === m ? 'mode-chip-active' : ''}`}>
                    <input type="radio" name="mode" checked={mode === m} onChange={() => setMode(m)} />
                    {m}
                  </label>
                ))}
              </div>
              <div className="flex flex-wrap gap-2">
                {['General', 'Fantasy'].map((c) => (
                  <button key={c} onClick={() => setSelected(c)} className={`choice-btn ${selected === c ? 'choice-btn-active' : ''}`}>
                    Choice: {c}
                  </button>
                ))}
              </div>
            </div>
          </Card>

          {/* Form */}
          <Card title="Form inputs">
            <div className="flex flex-col gap-3 mt-3">
              <label className="flex flex-col gap-1">
                <span className="label">Text input</span>
                <input className="field-input" placeholder="Character name" />
              </label>
              <label className="flex flex-col gap-1">
                <span className="label">Select</span>
                <select className="field-input">
                  <option>Option one</option>
                  <option>Option two</option>
                </select>
              </label>
              <label className="flex flex-col gap-1">
                <span className="label">Textarea</span>
                <textarea className="field-input" rows={3} placeholder="Notes…" />
              </label>
              <div className="alert-error">Error: something went wrong.</div>
              <div className="alert-success">Success: character saved.</div>
            </div>
          </Card>

          {/* Stat tiles */}
          <Card title="Stat tiles">
            <div className="grid grid-cols-3 gap-2 mt-3">
              <StatTile label="STR" value={14} />
              <StatTile label="DEX" value={12} />
              <StatTile label="CON" value={16} />
            </div>
          </Card>

          {/* Expandables + panels */}
          <Card title="Expandables & panels">
            <div className="flex flex-col gap-3 mt-3">
              <Expandable name="Darkvision" description="You can see in the dark up to 60 feet." />
              <Expandable name="No description" />
              <div className="rounded-lg border p-4 panel fg text-sm">Panel surface</div>
              <div className="rounded-lg border p-4 panel-danger fg-danger text-sm">Danger panel</div>
              <div className="rounded-lg border p-4 bg-soft border-subtle fg text-sm">Soft background</div>
              <div className="rounded-lg border p-4 bg-tint border-subtle fg text-sm">Tinted background</div>
            </div>
          </Card>
        </div>

        {/* Roll log */}
        <Card title="Roll attempts">
          <div className="flex flex-col gap-2 mt-3">
            <div className="roll-attempt p-3 text-sm fg">d20: 7 (not chosen)</div>
            <div className="roll-attempt roll-attempt-chosen p-3 text-sm fg">d20: 18 (chosen)</div>
          </div>
        </Card>

        <p className="empty-state">Empty state text</p>
      </div>
    </div>
  )
}
