import { IContractReviewItem } from "./contractReviewItem";
import { IJobDescription } from "./jobDescription";
import { IRemuneration } from "./remuneration";

export interface IOrderItem{
     selected: boolean;
     id: number; 
     orderId: number;
     orderNo: number; 
     srNo: number; 
     professionId: number;
     ecnr: boolean; 
     sourceFrom: string; 
     quantity: number; 
     minCVs: number; 
     maxCVs: number;
     //requireInternalReview: boolean; 
     //requireAssess: boolean; 
     completeBefore: Date; 
     //assignedId: number; 
     //assignedToName: string; 
     //charges: number; 
     //feeFromClientINR: number; 
     status: string;
     reviewItemStatus: string;
     checked: boolean;
     jobDescription: IJobDescription; 
     remuneration: IRemuneration;
     contractReviewItem: IContractReviewItem;
}