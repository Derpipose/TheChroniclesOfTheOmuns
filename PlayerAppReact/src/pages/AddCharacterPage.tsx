import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

export default function AddCharacterPage() {
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [charClass, setCharClass] = useState('')
  const [level, setLevel] = useState('1')

  const handleAdd = () => {
    // TODO: wire up to real state/backend
    navigate('/characters')
  }

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
      {/* Nav */}
      <div className="border-b px-6 py-4 flex items-center gap-3" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>
        <button
          onClick={() => navigate('/characters')}
          className="text-sm transition-colors hover:underline"
          style={{ color: '#7A6F5C' }}
        >
          ← Back
        </button>
        <h1 className="text-lg font-semibold" style={{ color: '#2C2416' }}>Add Character</h1>
      </div>

      <div className="max-w-md mx-auto px-6 py-8">
        <div className="rounded-2xl border p-6 flex flex-col gap-5" style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}>

          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
              Name
            </label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
              style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
              placeholder="Character name"
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
              Class
            </label>
            <input
              type="text"
              value={charClass}
              onChange={(e) => setCharClass(e.target.value)}
              className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
              style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
              placeholder="e.g. Fighter, Mage, Ranger"
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium uppercase tracking-wide" style={{ color: '#7A6F5C' }}>
              Level
            </label>
            <input
              type="number"
              min="1"
              max="20"
              value={level}
              onChange={(e) => setLevel(e.target.value)}
              className="rounded-lg px-3 py-2 text-sm outline-none border focus:border-stone-400 transition-colors"
              style={{ backgroundColor: '#F5F0E8', borderColor: '#DDD8CC', color: '#2C2416' }}
            />
          </div>

          <div className="flex gap-3 mt-2">
            <button
              onClick={() => navigate('/characters')}
              className="flex-1 rounded-lg py-2 text-sm font-medium transition-colors"
              style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}
            >
              Cancel
            </button>
            <button
              onClick={handleAdd}
              className="flex-1 rounded-lg py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
              style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
            >
              Add Character
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
