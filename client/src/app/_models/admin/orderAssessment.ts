import { IOrderItemAssessment } from "./orderItemAssessment";

export interface IOrderAssessment {
     id: number;
     orderAssessmentId: number;
     customerName: string;
     orderId: number;
     orderNo: number;
     orderItemAssessments: IOrderItemAssessment[];
}

export interface IAssessmentDto {
     id: number;
     orderAssessmentId: number;
     customerName: string;
     orderId: number;
     orderNo: number;
     
}