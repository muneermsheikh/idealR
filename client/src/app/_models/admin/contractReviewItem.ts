import { IContractReviewItemQ } from "src/app/_models/orders/contractReviewItemQ";

export interface IContractReviewItem
{
     id: number; 
     contractReviewId: number; 
     orderItemId: number; 
     professionName: string; 
     quantity: number;
     ecnr: boolean; 
     sourceFrom: string; 
     requireAssess: string; 
     charges: number;
     hrExecUsername: string;
     reviewItemStatus: string;
     contractReviewItemQs: IContractReviewItemQ[];

     
}