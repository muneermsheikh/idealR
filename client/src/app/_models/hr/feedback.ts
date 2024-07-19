export interface IFeedback
{
    id: number;
    feedbackNo: number;
    customerId: number;
    customerName: string;
    city: string;
    country: string;
    officialName: string;
    officialAppUserId: number;
    designation: string;
    email: string;
    phoneNo: string;
    dateIssued: Date;
    dateReceived?: Date;
    gradeAssessedByClient: string;
    customerSuggestion: string;
    feedbackItems: IFeedbackItem[];
}

export class Feedback implements IFeedback
{
    id=0;
    feedbackNo: number=0;
    customerId= 0;
    customerName='';
    city= '';
    country= '';
    officialName= '';
    officialAppUserId= 0;
    designation= '';
    email= '';
    phoneNo= '';
    dateIssued= new Date();
    dateReceived?=new (Date);
    gradeAssessedByClient= '';
    customerSuggestion= '';
    feedbackItems: IFeedbackItem[]=[];
}

export interface IFeedbackItem
{
    id: number;
    customerFeedbackId: number;
    feedbackGroup: string;
    questionNo: number;
    question: string;
    prompt1: string;
    prompt2: string;
    prompt3: string;
    prompt4: string;
    response: string;
    remarks: string;
}

//stdds
export interface IFeedbackQ
{
    id: number;
    feedbackGroup: string;
    questionNo: number;
    question: string;
    prompt1: string;
    prompt2: string;
    prompt3: string;
    prompt4: string;
}

//inputs
export interface IFeedbackInput
{
    id: number;
    feedbackId: number;
    customerId: number;
    customerName: string;
    city: string;
    officialName: string;
    designation: string;
    email: string;
    phoneNo: string;
    dateIssued: Date;
    gradeAssessedByClient: string;
    customerSuggestion: string;
    feedbackInputItems: IFeedbackInputItem[];
}


export class FeedbackInput implements IFeedbackInput
{
    id = 0;
    feedbackId=0;
    customerId=0;
    customerName='';
    city='';
    officialName='';
    designation='';
    email='';
    phoneNo='';
    dateIssued = new Date();
    gradeAssessedByClient='';
    customerSuggestion='';
    feedbackInputItems: IFeedbackInputItem[]=[];
}

export interface IFeedbackInputItem
{
    id: number;
    feedbackGroup: '';
    feedbackInputId: number;
    questionNo: number;
    question: string;
    prompt1: string;
    prompt2: string;
    prompt3: string;
    prompt4: string;
    response: string;
    remarks: string;
}

