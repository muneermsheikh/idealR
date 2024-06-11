export interface ICandidateAssessedShortDto
{
     candidateAssessmentId: number;
     orderItemId: number;
     customerName: string;
     requireInternalReview: boolean;
     candidateName: string;
     assessedByUsername: string;
     assessResult: string;
     assessedOn: Date;
     checklistHRId: number;
     categoryRef: string;
}

export class CandidateAssessedShortDto implements ICandidateAssessedShortDto
{
     candidateAssessmentId= 0;
     orderItemId= 0;
     customerName = '';
     requireInternalReview= false;
     candidateName= '';
     assessedByUsername= '';
     checklistHRId= 0;
     assessResult= '';
     assessedOn: Date = new Date();

     categoryRef = '';
}