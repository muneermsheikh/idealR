import { IContractReviewItemQ } from "src/app/_models/orders/contractReviewItemQ";


export interface IContractReviewItemDto
{
    id: number; 
    
    orderId: number; 
    orderNo: number;
    orderDate: Date;
    customerName: string;

    contractReviewId: number; 
    orderItemId: number; 
    professionName: string; 
    quantity: number;
    ecnr: boolean; 
    sourceFrom: string; 
    requireAssess: boolean; 
    charges: number;
    hrExecUsername: string;
    reviewItemStatus: string;
   
    contractReviewItemQs: IContractReviewItemQ[];
}