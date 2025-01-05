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

export class CandidateAssessmentDto implements ICandidateAssessmentDto
{
     id= 0;
     candidateId= 0;
     candidateName= '';
     applicationNo= 0;

     customerName= '';
     professionName= '';
     categoryRefAndName= '';
     orderDate = new Date;

     orderItemId= 0;
     categoryRef= '';
     orderId= 0;
     //professionName= '';
     requireInternalReview = false;
     assessedByUsername= '';
     assessResult= '';
     assessedOn = new Date;
     checklistHRId= 0;
     checklistedByName= '';
     checklistedOn = new Date;
    
     assessmentItemsDto: IAssessmentItemDto[] = [];
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

export class AssessmentItemDto implements IAssessmentItemDto
{
     id = 0;
     candidateAssessmentId = 0;
     questionNo=0;
     isMandatory = false;
     assessed = false;
     question = '';
     maxPoints = 0;
     points = 0;
     remarks = '';
}