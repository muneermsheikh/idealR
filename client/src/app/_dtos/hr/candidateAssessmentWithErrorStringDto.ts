import { ICandidateAssessment } from "src/app/_models/hr/candidateAssessment";


export interface ICandidateAssessmentWithErrorStringDto
{
     candidateAssessment: ICandidateAssessment;
     errorString: string;
}