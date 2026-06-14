import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

type Tab = 'sign-in' | 'sign-up'

export default function SignInPage() {
  const [tab, setTab] = useState<Tab>('sign-in')
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()

  const handleSubmit = () => {
    navigate('/characters')
  }

  return (
    <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: '#F5F0E8' }}>
      <div className="w-full max-w-sm">
        <h1 className="text-3xl font-semibold text-center mb-8" style={{ color: '#2C2416' }}>
          Character Vault
        </h1>

        <div className="rounded-2xl shadow-sm border overflow-hidden" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
          {/* Tabs */}
          <div className="flex" style={{ borderBottom: '1px solid #DDD8CC' }}>
            {(['sign-in', 'sign-up'] as Tab[]).map((t) => (
              <button
                key={t}
                onClick={() => setTab(t)}
                className="flex-1 py-3 text-sm font-medium transition-colors"
                style={{
                  backgroundColor: tab === t ? '#FDFAF4' : '#EDE8DC',
                  color: tab === t ? '#2C2416' : '#7A6F5C',
                  borderBottom: tab === t ? '2px solid #8B7355' : '2px solid transparent',
                }}
              >
                {t === 'sign-in' ? 'Sign In' : 'Sign Up'}
              </button>
            ))}
          </div>

          {/* Form */}
          <div className="p-6 flex flex-col gap-4">
            <div className="flex flex-col gap-1">
              <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                Username
              </label>
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
                style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                placeholder="your_username"
              />
            </div>

            <div className="flex flex-col gap-1">
              <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                Password
              </label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
                style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                placeholder="••••••••"
              />
            </div>

            <button
              onClick={handleSubmit}
              className="mt-2 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
              style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
            >
              {tab === 'sign-in' ? 'Sign In' : 'Create Account'}
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
