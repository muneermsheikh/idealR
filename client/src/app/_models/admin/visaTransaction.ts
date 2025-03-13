export interface IVisaTransaction
{
    id: number;
    customerKnownAs: string;
    applicationNo: number;
    candidateName: string;
    cvRefId: number;
    visaItemId: number;
    visaNo: string;
    visaCategory: string;
    depItemId: number;
    visaAppSubmitted: Date;
    visaApproved: Date;
    visaDateG: Date;
    customerId: number;
}