export type SeedStatus = 'idle' | 'loading' | 'success' | 'error'

export interface SeedResult {
  talents: SeedStatus
  classes: SeedStatus
  backgrounds: SeedStatus
  races: SeedStatus
  subraces: SeedStatus
}

// Shape returned by all three ScifiSeedingController endpoints.
export interface SeedResponse {
  created: number
  updated: number
  total: number
  skippedClasses?: string[]
  skippedTalents?: string[]
}