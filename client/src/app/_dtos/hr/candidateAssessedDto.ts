export interface ICandidateAssessedDto
{
     id: number;
     checked: boolean;
     orderItemId: number;
     orderId: number;
     professionName: string;
     applicationNo: number;
     customerName: string;
     requireInternalReview: boolean;
     candidateId: number;
     candidateName: string;
     assessedByUsername: string;
     assessResult: string;
     assessedOn: Date;
     checklistHRId: number;
     checklistedByName: string;
     checklistedOn: Date;
     categoryRef: string;
     assessedByName: string;
}

export class CandidateAssessedDto implements ICandidateAssessedDto
{
     id= 0;
     checked= false;
     orderItemId= 0;
     orderId=0;
     professionName= '';
     applicationNo= 0;
     customerName= '';
     requireInternalReview= false;
     candidateId= 0;
     candidateName= '';
     assessedByUsername= '';
     checklistHRId= 0;
     assessResult= '';
     assessedOn: Date = new Date();

     checklistedByName = '';
     checklistedOn = new Date();
     categoryRef = '';
     assessedByName = '';
}