
export interface ScifiClass{
    id: number;
    name: string;
    description: string;
}

export interface ScifiBackground{
    id: number;
    name: string;
    description: string;
}

export interface ScifiRace{
    id: number;
    name: string;
    raceStats: RaceStats;
    description: string;
    hasSubraces: boolean;
}

export interface ScifiSubrace{
    id: number;
    subRaceName: string;
    raceStats: RaceStats;
    description: string;
    scifiRaceId: number;
}

export interface RaceStats{
    id: number;
    modifiers: RaceStatModifier[];
    freeStatPicks: number[];
}

export interface RaceStatModifier{
    stat: string;
    modifier: number;
    isLocked: boolean;
}