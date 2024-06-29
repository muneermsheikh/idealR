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
    phoneNo: string;
    email: string;
    currentLocation: string;
    workExperience: number;
    status?: string;
    userName: string;
    statusDate: Date;
    remarks: string;
}