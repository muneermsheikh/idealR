export interface IMessageToSendDto
{
    id: number;
    senderUsername: string;
    recipientUsername: string;
    ccEmailAddress: string;
    subject: string;
    content: string;
}