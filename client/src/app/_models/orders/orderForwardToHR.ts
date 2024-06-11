export interface IOrderForwardToHR
{
    orderId: number;
    recipientUsername: string;
    dateForwarded: Date;
}

export class OrderForwardToHR implements IOrderForwardToHR
{
    orderId=0;
    recipientUsername = '';
    dateForwarded= new Date();
}