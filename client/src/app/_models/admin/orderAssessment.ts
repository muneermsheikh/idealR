import { IOrderAssessmentItem } from "./orderAssessmentItem";


export interface IOrderAssessment {
     id: number;
     orderAssessmentId: number;
     customerName: string;
     orderId: number;
     orderNo: number;
     dateDesigned: Date;
     orderAssessmentItems: IOrderAssessmentItem[];
}

export interface IOrderAssessmentDto {
     id: number;
     orderAssessmentId: number;
     customerName: string;
     orderId: number;
     orderNo: number;
     dateDesigned: Date;
}