export interface IProspectiveBriefDto
{
    id: number;
    checked: boolean;
    source: string;
    personType: string;
    personId: string;
    nationality: string;
    dateRegistered: Date;
    candidateName: string;
    categoryRef: string;
    customerName: string;
    phoneNo: string;
    email: string;
    currentLocation: string;
    workExperience: number;
    status?: string;
    bySMS: boolean;
    byMail: boolean;
    byPhone: boolean;
    userName: string;
    statusDate: Date;
    remarks: string;
}