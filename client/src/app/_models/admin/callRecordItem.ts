export interface ICallRecordItem 
{
     id: number;
     incomingOutgoing: string;
     callRecordId: number;
     phoneNo: string;
     email: string;
     dateOfContact: Date;
     username: string;
     contactResult: string;
     gistOfDiscussions: string;
     nextAction: string;
     nextActionOn: Date;
     advisoryBy: string
}

export class CallRecordItem implements ICallRecordItem 
{
     id = 0;
     incomingOutgoing = "in";
     phoneNo = '';
     email='';
     callRecordId = 0;
     dateOfContact = new Date();
     username = '';
     contactResult = '';
     gistOfDiscussions = '';
     nextAction="";
     nextActionOn= new Date;
     advisoryBy="";
}