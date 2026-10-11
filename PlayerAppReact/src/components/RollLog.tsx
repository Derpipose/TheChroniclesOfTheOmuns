import { useEffect, useRef } from 'react'

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
  onVisibilityChange?: (isVisible: boolean) => void
}

export default function RollLog({ rolls, onVisibilityChange }: RollLogProps) {
  const rollLogRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!onVisibilityChange || !rollLogRef.current) return

    const observer = new IntersectionObserver(
      ([entry]) => onVisibilityChange(entry.isIntersecting),
      { threshold: 0.1 },
    )

    observer.observe(rollLogRef.current)
    return () => observer.disconnect()
  }, [onVisibilityChange])

  return (
    <div
      ref={rollLogRef}
      className="roll-log"
    >
      <div className="space-y-3">
        {rolls.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-sm fg-muted">
              No rolls yet
            </p>
          </div>
        ) : (
          rolls.map((roll) => (
            <div
              key={roll.id}
              className="rounded-lg border p-3 sm:p-4 panel"
            >
              <div className="flex flex-wrap items-start justify-between gap-3 sm:gap-4 mb-2">
                <div className="flex-1 min-w-0">
                  {roll.label && (
                    <p className="text-sm font-semibold mb-1 fg">
                      {roll.label}
                    </p>
                  )}
                  <p className="text-xs fg-muted">
                    {roll.timestamp}
                  </p>
                </div>
                <div className="ml-auto text-right">
                  {roll.attempts ? (
                    <div className="flex flex-wrap justify-end gap-1.5 sm:gap-2">
                      {roll.attempts.map((attempt, index) => (
                        <div
                          key={`${roll.id}-summary-${index}`}
                            className={`roll-attempt px-1.5 py-1 sm:px-2 ${index === roll.chosenAttemptIndex ? 'roll-attempt-chosen' : ''}`}
                        >
                          <p className="text-xl font-bold font-mono sm:text-2xl fg-accent">
                            {attempt.total}
                          </p>
                          <p className="text-xs fg-muted">
                            {index === roll.chosenAttemptIndex ? 'Chosen' : `Roll ${index + 1}`}
                          </p>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="text-xl font-bold font-mono sm:text-2xl fg-accent">
                      {roll.total}
                    </p>
                  )}
                </div>
              </div>

              <div className="space-y-1 pt-2 border-t border-subtle">
                <p className="text-xs font-mono fg">
                  {roll.expression}
                </p>
                {roll.attempts && (
                  <div className="space-y-2 pt-1">
                    <p className="text-xs capitalize fg-muted">
                      {roll.rollMode}
                    </p>
                    {roll.attempts.map((attempt, index) => (
                      <div
                        key={`${roll.id}-attempt-${index}`}
                        className={`roll-attempt px-2 py-1 ${index === roll.chosenAttemptIndex ? 'roll-attempt-chosen' : ''}`}
                      >
                        <p className="text-xs font-mono fg">
                          Attempt {index + 1}: {attempt.total}
                          {index === roll.chosenAttemptIndex && ' — Chosen'}
                        </p>
                        <p className="text-xs font-mono fg-muted">
                          [{attempt.allRolls.join(', ')}]
                        </p>
                      </div>
                    ))}
                  </div>
                )}
                <p className="text-xs font-mono fg-muted">
                  = {roll.resolvedExpr}
                </p>
                <p className="text-xs font-mono break-all fg-muted">
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
