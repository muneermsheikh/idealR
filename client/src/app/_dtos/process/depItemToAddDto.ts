export interface IDepItemToAddDto
{
    depId: number;
    transactionDate: Date;
    sequence: number;
}

export class DepItemToAddDto
{
    depId = 0;
    transactionDate = new Date();
    sequence = 0;
}