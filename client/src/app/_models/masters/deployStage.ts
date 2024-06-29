export interface IDeployStage
{
     id: number;
     sequence: number;
     status: string;
     estimatedDaysToCompleteThisStage: number;
     nextSequence: number;
     isOptional: boolean;
}

export interface IDeployStatusAndName
{
     id: number;
     name: string;
}