import { IContractReviewItemQ } from "src/app/_models/orders/contractReviewItemQ";


export interface IContractReviewItemDto
{
    id: number; 
    contractReviewId: number; 
    orderItemId: number; 
    orderId: number; 
    professionName: string; 
    quantity: number;
    ecnr: boolean; 
    sourceFrom: string; 
    requireAssess: boolean; 
    charges: number;
    hrExecUsername: string;
    reviewItemStatus: string;
    contractReviewItemQs: IContractReviewItemQ[];

    orderDate: Date;
    customerName: string;
    orderNo: number;
    
}