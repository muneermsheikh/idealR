export interface IDeploymentStatus
{
     id: number;
     sequence: number;
     statusName: string;
     workingDaysReqdForNextStage: number;
     nextSequence: number;
     isOptional: boolean;
}
