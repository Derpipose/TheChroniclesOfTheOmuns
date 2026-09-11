import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { setToken } from '../auth/tokenStorage'

type Tab = 'sign-in' | 'sign-up'
type Step = 'email' | 'otp'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5237'

export default function SignInPage() {
  const [tab, setTab] = useState<Tab>('sign-in')
  const [step, setStep] = useState<Step>('email')
  const [email, setEmail] = useState('')
  const [displayName, setDisplayName] = useState('')
  const [otp, setOtp] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

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

      setStep('otp')
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
      setToken(data.token) // Store the token in localStorage using the setToken function
      navigate('/characters')
    } catch {
      setError('Something went wrong. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  const handleTabChange = (t: Tab) => {
    setTab(t)
    setStep('email')
    setError(null)
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      step === 'email' ? handleRequestOtp() : handleVerifyOtp()
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
            {(['sign-in', 'sign-up'] as Tab[]).map((t) => (
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
                {t === 'sign-in' ? 'Sign In' : 'Sign Up'}
              </button>
            ))}
          </div>

          {/* Form */}
          <div className="p-6 flex flex-col gap-4">
            {step === 'email' ? (
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
                      className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
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
                    className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
                    style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                    placeholder="you@example.com"
                  />
                </div>
                {error && <p className="text-xs" style={{ color: '#B04A2F' }}>{error}</p>}
                <button
                  onClick={handleRequestOtp}
                  disabled={loading}
                  className="mt-2 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50"
                  style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
                >
                  {loading ? 'Sending...' : tab === 'sign-in' ? 'Send Code' : 'Create Account'}
                </button>
              </>
            ) : (
              <>
                <p className="text-sm" style={{ color: '#7A6F5C' }}>
                  A login code was sent to <strong style={{ color: '#2C2416' }}>{email}</strong>
                </p>
                <div className="flex flex-col gap-1">
                  <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
                    Login Code
                  </label>
                  <input
                    type="text"
                    value={otp}
                    onChange={(e) => setOtp(e.target.value)}
                    onKeyDown={handleKeyDown}
                    className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
                    style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
                    placeholder="123456"
                  />
                </div>
                {error && <p className="text-xs" style={{ color: '#B04A2F' }}>{error}</p>}
                <button
                  onClick={handleVerifyOtp}
                  disabled={loading}
                  className="mt-2 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75 disabled:opacity-50"
                  style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
                >
                  {loading ? 'Verifying...' : 'Sign In'}
                </button>
                <button
                  onClick={() => { setStep('email'); setError(null) }}
                  className="text-xs text-center transition-colors hover:underline"
                  style={{ color: '#7A6F5C' }}
                >
                  ← Use a different email
                </button>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
