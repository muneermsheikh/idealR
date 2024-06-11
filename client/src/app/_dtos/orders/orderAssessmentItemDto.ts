import { IOrderAssessmentItemQ } from "src/app/_models/admin/orderAssessmentItemQ";

export interface IOrderAssessmentItemDto
{
    id: number;
    orderAssessmentId: number;
    orderItemId: number;
    orderId: number;
    orderNo:  number;
    customerName: string;
    professionId: number;
    professionName: string;
    orderAssessmentItemQs: IOrderAssessmentItemQ[];
}