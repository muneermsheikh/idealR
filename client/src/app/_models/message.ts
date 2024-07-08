export interface Message
{
    id: number;
    cvRefId: number;
    senderAppUserId: number;
    senderUsername: string;
    senderEmail: string;
    //senderPhotoUrl: string;
    recipientAppUserId: number;
    recipientUsername: string;
    recipientEmail: string;
    subject: string;
    messageComposedOn: Date;
    //recipientPhotoUrl: string;

    content: string;
    //dateRead?: Date;
    messageSent: Date
}