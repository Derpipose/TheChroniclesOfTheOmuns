interface DiceRollAttempt {
  resolvedExpr: string
  allRolls: number[]
  total: number
}

export type RollMode = 'standard' | 'advantage' | 'disadvantage'

export interface DiceRoll {
  id: string
  timestamp: string
  expression: string
  resolvedExpr: string
  allRolls: number[]
  total: number
  label?: string
  attempts?: DiceRollAttempt[]
  rollMode?: RollMode
  chosenAttemptIndex?: number
}

interface RollLogProps {
  rolls: DiceRoll[]
}

export default function RollLog({ rolls }: RollLogProps) {
  return (
    <div
      className="w-2/3 overflow-y-auto p-6"
      style={{ backgroundColor: '#F5F0E8' }}
    >
      <div className="space-y-3">
        {rolls.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-sm" style={{ color: '#7A6F5C' }}>
              No rolls yet
            </p>
          </div>
        ) : (
          rolls.map((roll) => (
            <div
              key={roll.id}
              className="rounded-lg border p-4"
              style={{
                backgroundColor: '#FDFAF4',
                borderColor: '#DDD8CC',
              }}
            >
              <div className="flex items-start justify-between gap-4 mb-2">
                <div className="flex-1 min-w-0">
                  {roll.label && (
                    <p className="text-sm font-semibold mb-1" style={{ color: '#2C2416' }}>
                      {roll.label}
                    </p>
                  )}
                  <p className="text-xs" style={{ color: '#7A6F5C' }}>
                    {roll.timestamp}
                  </p>
                </div>
                <div className="text-right">
                  {roll.attempts ? (
                    <div className="flex gap-2">
                      {roll.attempts.map((attempt, index) => (
                        <div
                          key={`${roll.id}-summary-${index}`}
                          className="rounded border px-2 py-1"
                          style={{
                            borderColor: index === roll.chosenAttemptIndex ? '#8B7355' : '#DDD8CC',
                            backgroundColor: index === roll.chosenAttemptIndex ? '#F3EBDD' : '#FDFAF4',
                          }}
                        >
                          <p className="text-2xl font-bold font-mono" style={{ color: '#8B7355' }}>
                            {attempt.total}
                          </p>
                          <p className="text-xs" style={{ color: '#7A6F5C' }}>
                            {index === roll.chosenAttemptIndex ? 'Chosen' : `Roll ${index + 1}`}
                          </p>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="text-2xl font-bold font-mono" style={{ color: '#8B7355' }}>
                      {roll.total}
                    </p>
                  )}
                </div>
              </div>

              <div className="space-y-1 pt-2 border-t" style={{ borderColor: '#DDD8CC' }}>
                <p className="text-xs font-mono" style={{ color: '#2C2416' }}>
                  {roll.expression}
                </p>
                {roll.attempts && (
                  <div className="space-y-2 pt-1">
                    <p className="text-xs capitalize" style={{ color: '#7A6F5C' }}>
                      {roll.rollMode}
                    </p>
                    {roll.attempts.map((attempt, index) => (
                      <div
                        key={`${roll.id}-attempt-${index}`}
                        className="rounded border px-2 py-1"
                        style={{
                          borderColor: index === roll.chosenAttemptIndex ? '#8B7355' : '#DDD8CC',
                          backgroundColor: index === roll.chosenAttemptIndex ? '#F3EBDD' : '#FDFAF4',
                        }}
                      >
                        <p className="text-xs font-mono" style={{ color: '#2C2416' }}>
                          Attempt {index + 1}: {attempt.total}
                          {index === roll.chosenAttemptIndex && ' — Chosen'}
                        </p>
                        <p className="text-xs font-mono" style={{ color: '#7A6F5C' }}>
                          [{attempt.allRolls.join(', ')}]
                        </p>
                      </div>
                    ))}
                  </div>
                )}
                <p className="text-xs font-mono" style={{ color: '#7A6F5C' }}>
                  = {roll.resolvedExpr}
                </p>
                <p className="text-xs font-mono break-all" style={{ color: '#7A6F5C' }}>
                  [{roll.allRolls.join(', ')}]
                </p>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  )
}
