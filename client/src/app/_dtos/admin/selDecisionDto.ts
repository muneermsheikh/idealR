export interface ISelDecisionDto
{
    id: number;
    cvRefId: number;
    orderItemId: number;
    professionId: number;
    candidateId: number;
    candidateName: string;
    applicationNo: number;

    customerName: string;
    categoryRef: string;
    referredOn: Date;
    selectedOn: Date;
    selectionStatus: string;
    //rejectionReason: string;
    
}