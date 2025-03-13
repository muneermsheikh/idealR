export interface INextDepDataDto
{
    candidateName: string;
    applicationNo: number;
    errorString: string;
    referrals: IReferral[];
}

export interface IReferral
{
    depId: number;
    categoryRef: string;
    customerName: string;
    referredOn: Date;
    sequence: number;
    sequenceName: string;
    period: number;
}