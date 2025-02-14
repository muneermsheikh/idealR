export interface IAudioMessage
{
    subject: string;
    recipientUsername: string;
    senderUsername: string;
    candidateName: string;
    applicationNo: number;
    messageText: string;
    dateComposed: Date;
    datePlayedback: Date;
    fileName: string;
    contentType: string;
    feedbackReceived: number;
}