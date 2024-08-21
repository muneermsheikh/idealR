export interface IDepItem
{
    id: number;
    depId: number;
    transactionDate: Date;
    sequence: number;
    nextSequence: number;
    nextSequenceDate: Date;
    fullPath: string;
}

export class DepItem implements IDepItem
{
    id=0;
    depId=0;
    transactionDate = new Date();
    sequence = 0;
    nextSequence = 0;
    nextSequenceDate = new Date();
    fullPath = "";
}