import { ICandidateAssessmentItem } from "./candidateAssessmentItem";

export interface ICandidateAssessment
{
     id: number;
     orderItemId: number;
     candidateId: number;
     
     customerName: string;
     categoryRefAndName: string;
     orderDate: Date;

     assessedByEmployeeName: string;
     assessedOn: Date;
     assessResult: string;
     requireInternalReview: boolean;
     checklistHRId: number;
     taskIdDocControllerAdmin: number;
     remarks: string;
     candidateAssessmentItems: ICandidateAssessmentItem[];
}
export class CandidateAssessment implements ICandidateAssessment
{
   
     id= 0;
     orderItemId =0;

     customerName='';
     categoryRefAndName = '';
     orderDate = new Date();
     
     candidateId= 0;
     assessedByEmployeeName= '';
     assessedOn= new Date();
     assessResult= "";
     requireInternalReview = false;
     checklistHRId= 0;
     remarks= '';
     cvRefId= 0;
     taskIdDocControllerAdmin= 0;
     candidateAssessmentItems: ICandidateAssessmentItem[]=[];
}
