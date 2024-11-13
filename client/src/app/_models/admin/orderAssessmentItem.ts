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

export class OrderAssessmentItem implements IOrderAssessmentItem
{
     id= 0;
     orderAssessmentId= 0;
     assessmentRef= '';
     orderItemId= 0;
     customerName= '';
     professionId= 0;
     professionName= '';
     orderId= 0;
     orderNo= 0;
     approvedBy= '';
     dateDesigned = new Date;     
     requireCandidateAssessment = false;
     designedBy= '';
     orderAssessmentItemQs: IOrderAssessmentItemQ[] = [];
}