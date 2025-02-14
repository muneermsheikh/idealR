export interface IAudioMessageDto
{
    checked: boolean;
    id: number;
    applicationNo: number;
    candidateName: string;
    messageText: string;
    subject: string;
    dateComposed: Date;
    datePlayedback: Date;
    fileName: string;
    feedbackReceived: number;
}