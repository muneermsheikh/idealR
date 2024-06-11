import { IOrderAssessmentItemQ } from "./orderAssessmentItemQ";


export interface IOrderAssessmentItem
{
     id: number;
     orderAssessmentId: number;
     orderItemId: number;
     orderId: number;
     orderNo: number;
     customerName: string;
     professionId: number;
     professionName: string;
     designedBy: string;
     approvedBy: string;
     orderAssessmentItemQs: IOrderAssessmentItemQ[];
}