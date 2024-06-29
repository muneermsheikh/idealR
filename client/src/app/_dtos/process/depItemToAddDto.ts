export interface IDepItemToAddDto
{
    id: number;
    depId: number;
    transactionDate: Date;
    sequence: number;
    nextSequence: number;
}

export class DepItemToAddDto
{
    id = 0;
    depId = 0;
    transactionDate = new Date();
    sequence = 0;
    nextSequence = 0;
}