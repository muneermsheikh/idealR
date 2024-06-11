import { IContractReviewItemQ } from "src/app/_models/orders/contractReviewItemQ";


export interface IContractReviewItemDto
{
    id: number; 
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
   
    orderId: number; 
    orderNo: number;
    orderDate: Date;
    customerName: string;

    contractReviewItemQs: IContractReviewItemQ[];
}