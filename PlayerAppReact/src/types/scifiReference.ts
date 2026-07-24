export interface ScifiRace{
    id: number;
    name: string;
    description: string;
    hasSubrace: boolean;
}

export interface ScifiSubrace{
    id: number;
    subRaceName: string;
    description: string;
    scifiRaceId: number;
}

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