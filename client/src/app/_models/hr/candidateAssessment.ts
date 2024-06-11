import { ICandidateAssessmentItem } from "./candidateAssessmentItem";

export interface ICandidateAssessment
{
     id: number;
     orderItemId: number;
     candidateId: number;
     
     customerName: string;
     categoryRefAndName: string;
     orderDate: Date;

     assessedByName: string;
     assessedOn: Date;
     assessResult: string;
     requireInternalReview: string;
     hrChecklistId: number;
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
     assessedByName= '';
     assessedOn= new Date();
     assessResult= "";
     requireInternalReview = 'N';
     hrChecklistId= 0;
     remarks= '';
     cvRefId= 0;
     taskIdDocControllerAdmin= 0;
     candidateAssessmentItems: ICandidateAssessmentItem[]=[];
}
