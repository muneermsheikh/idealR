export interface IDepItemToAddDto
{
    id: number;
    depId: number;
    transactionDate: Date;
    sequence: number;

    sequenceName: string;
    nextSequence: number;
    fullPath: string;
}

export class DepItemToAddDto
{
    id = 0;
    depId = 0;
    transactionDate = new Date();
    sequence = 0;
    
    sequenceName = '';
    nextSequence = 0;
    fullPath = '';
}