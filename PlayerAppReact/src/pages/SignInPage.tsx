import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

type Tab = 'send' | 'enter' | 'sign-up'

const TAB_LABELS: Record<Tab, string> = {
  'send': 'Send Code',
  'enter': 'Enter Code',
  'sign-up': 'Sign Up',
}

const EMAIL_KEY = 'signin-email'

function loadEmail(): string {
  try {
    return sessionStorage.getItem(EMAIL_KEY) ?? ''
  } catch {
    return ''
  }
}

function saveEmail(value: string) {
  try {
    sessionStorage.setItem(EMAIL_KEY, value)
  } catch {
    // ignore
  }
}

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export default function SignInPage() {
  const [tab, setTab] = useState<Tab>('send')
  const [email, setEmailState] = useState(loadEmail)
  const [displayName, setDisplayName] = useState('')
  const [otp, setOtp] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()
  const { signIn } = useAuth()

  const setEmail = (value: string) => {
    setEmailState(value)
    saveEmail(value)
  }

  const handleRequestOtp = async () => {
    setError(null)
    setLoading(true)
    try {
      if (tab === 'sign-up') {
        const res = await fetch(`${BASE_URL}/api/player/register`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ displayName, email }),
        })
        if (!res.ok) {
          const data = await res.json()
          setError(data.message ?? 'Registration failed.')
          return
        }
      }

      await fetch(`${BASE_URL}/auth/request`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email }),
      })

      setTab('enter')
    } catch {
      setError('Something went wrong. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  const handleVerifyOtp = async () => {
    setError(null)
    setLoading(true)
    try {
      const res = await fetch(`${BASE_URL}/auth/verify`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, otp }),
      })

      if (!res.ok) {
        setError('Invalid or expired code.')
        return
      }

      const data = await res.json()
      signIn(data.token)
      navigate('/characters')
    } catch {
      setError('Something went wrong. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  const handleTabChange = (t: Tab) => {
    setTab(t)
    setError(null)
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      tab === 'enter' ? handleVerifyOtp() : handleRequestOtp()
    }
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
            {(['send', 'enter', 'sign-up'] as Tab[]).map((t) => (
              <button
                key={t}
                onClick={() => handleTabChange(t)}
                className="flex-1 py-3 text-sm font-medium transition-colors"
                style={{
                  backgroundColor: tab === t ? '#FDFAF4' : '#EDE8DC',
                  color: tab === t ? '#2C2416' : '#7A6F5C',
                  borderBottom: tab === t ? '2px solid #8B7355' : '2px solid transparent',
                }}
              >
                {TAB_LABELS[t]}
              </button>
            ))}
          </div>

          {/* Form */}
          <div className="p-6 flex flex-col gap-4">
            {tab !== 'enter' ? (
              <>
                {tab === 'sign-up' && (
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                      Display Name
                    </label>
                    <input
                      type="text"
                      value={displayName}
                      onChange={(e) => setDisplayName(e.target.value)}
                      onKeyDown={handleKeyDown}
                      className="field-input"
                      style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                      placeholder="Your name"
                    />
                  </div>
                )}
                <div className="flex flex-col gap-1">
                  <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                    Email
                  </label>
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    onKeyDown={handleKeyDown}
                    className="field-input"
                    style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                    placeholder="you@example.com"
                  />
                </div>
                {error && <p className="text-xs" style={{ color: '#B04A2F' }}>{error}</p>}
                <button
                  onClick={handleRequestOtp}
                  disabled={loading}
                  className="btn mt-2"
                  style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
                >
                  {loading ? 'Sending...' : tab === 'send' ? 'Send Code' : 'Create Account'}
                </button>
              </>
            ) : (
              <>
                <div className="flex flex-col gap-1">
                  <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                    Email
                  </label>
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    onKeyDown={handleKeyDown}
                    className="field-input"
                    style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                    placeholder="you@example.com"
                  />
                </div>
                <div className="flex flex-col gap-1">
                  <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                    Login Code
                  </label>
                  <input
                    type="text"
                    value={otp}
                    onChange={(e) => setOtp(e.target.value)}
                    onKeyDown={handleKeyDown}
                    className="field-input"
                    style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                    placeholder="123456"
                  />
                </div>
                {error && <p className="text-xs" style={{ color: '#B04A2F' }}>{error}</p>}
                <button
                  onClick={handleVerifyOtp}
                  disabled={loading}
                  className="btn mt-2"
                  style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
                >
                  {loading ? 'Verifying...' : 'Sign In'}
                </button>
                <button
                  onClick={() => handleTabChange('send')}
                  className="text-xs text-center transition-colors hover:underline"
                  style={{ color: '#7A6F5C' }}
                >
                  Need a new code?
                </button>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
