export interface ICandidateAssessmentDto
{
     id: number;
     candidateId: number;
     candidateName: string;
     applicationNo: number;

     customerName: string;
     professionName: string;
     categoryRefAndName: string;
     orderDate: Date;

     orderItemId: number;
     categoryRef: string;
     orderId: number;
     //professionName: string;
     requireInternalReview: boolean;
     assessedByUsername: string;
     assessResult: string;
     assessedOn: Date;
     checklistHRId: number;
     checklistedByName: string;
     checklistedOn: Date;
    
     assessmentItemsDto: IAssessmentItemDto[]
}


export interface IAssessmentItemDto
{
     id: number;
     candidateAssessmentId: number;
     questionNo: number;
     isMandatory: boolean;
     assessed: boolean;
     question: string;
     maxPoints: number;
     points: number;
     remarks: string;
}