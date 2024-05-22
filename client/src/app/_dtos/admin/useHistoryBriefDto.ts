export interface IUserHistoryBriefDto
{
    id: number;
    gender: string;
    checked: boolean;
    source: string;
    categoryRef: string;
    categoryName: string;
    candidateName: string;
    applicationNo?: number;
    resumeId: string;
    emailId: string;
    mobileNo:string;
    createdOn: Date;
    status: string;
 }