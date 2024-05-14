import { IAssessmentQ } from "./assessmentQ";

export interface IAssessment {
     id: number;
     orderAssessmentId: number;
     orderItemId: number;
     orderId: number;
     orderNo: number;
     categoryId: number;
     categoryName: string;
     orderItemAssessmentQs: IAssessmentQ[];
}

export interface IAssessmentDto {
     id: number;
     orderAssessmentId: number;
     customerName: string;
     orderItemId: number;
     orderId: number;
     orderNo: number;
     categoryId: number;
     categoryName: string;
     
}