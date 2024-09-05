import { IOrderAssessmentItemQ } from "./orderAssessmentItemQ";


export interface IOrderAssessmentItem
{
     id: number;
     orderAssessmentId: number;
     assessmentRef: string;
     orderItemId: number;
     customerName: string;
     professionId: number;
     professionName: string;
     orderId: number;
     orderNo: number;
     approvedBy: string;
     dateDesigned: Date;     
     requireCandidateAssessment: boolean;
     designedBy: string;
     orderAssessmentItemQs: IOrderAssessmentItemQ[];
}