export interface ICallRecordParams
{
    id: number;
    personType: string;
    incomingOutgoing: string;
    dateOfContact: Date;
    personId: string;
    mobileNo: string;
    emailId: string;
    categoryRef:string;
    status: string;
    statusClass: string;
    subject: string;
    username: string;
    gistOfDiscussions: string;
    advisoryBy: string;
    nextAction:string;
    nextActionOn: Date;

    search: string;
    sort: string;
    pageNumber: number;
    pageSize: number;
}


export class CallRecordParams implements ICallRecordParams
{
    id = 0;
    personType = '';
    incomingOutgoing='';
    dateOfContact=new Date();
    personId="";
    mobileNo = '';
    emailId = '';
    categoryRef = '';
    status = '';
    statusClass='Active';
    username =  '';
    gistOfDiscussions='';
    advisoryBy='';
    nextAction='';
    nextActionOn=new Date();
    subject='';

    search = '';
    sort = '';
    pageNumber=1;
    pageSize=10;
}

