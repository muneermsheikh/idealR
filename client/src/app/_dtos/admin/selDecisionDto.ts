export interface ISelDecisionDto
{
    selDecisionId: number;
    cVRefId: number;
    orderItemId: number;
    candidateName: string;
    applicationNo: number;

    customerName: string;
    categoryRef: string;
    referredOn: Date;
    selectedOn: Date;
    selectionStatus: string;
    //rejectionReason: string;
    
}