export interface IFeedbackDto
{
    id:number;
    customerId:number;
    customerName:string;
    city:string;
    officialName:string;
    email:string;
    phoneNo:string;
    dateReceived: Date;
    dateIssued:Date;
    gradeAssessedByClient:string;
    customerSuggestion: string;
}

export class FeedbackDto
{
    id=0;
    customerId=0;
    customerName='';
    city='';
    officialName='';
    email='';
    phoneNo='';
    dateReceived=new Date();
    dateIssued=new Date();
    gradeAssessedByClient='';
    customerSuggestion='';
}