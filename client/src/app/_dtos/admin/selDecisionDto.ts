export interface ISelDecisionDto
{
    id: number;
    checked: boolean;
    cvRefId: number;
    orderItemId: number;
    orderId: number;
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