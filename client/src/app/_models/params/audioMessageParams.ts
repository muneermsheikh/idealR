export interface IAudioMessageParams
{
    recipientUsername: string;
    senderUsername: string;
    candidateName: string;
    applicationNo: number;
    dateComposed: Date;
    feedbackReceived: number;
    pageNumber: number;
    pageSize: number;
}

export class audioMessageParams implements IAudioMessageParams
{
    recipientUsername = '';
    senderUsername = '';
    candidateName = '';
    applicationNo = 0;
    dateComposed= new Date();
    feedbackReceived = 0;
    pageNumber=1;
    pageSize=15;
}
