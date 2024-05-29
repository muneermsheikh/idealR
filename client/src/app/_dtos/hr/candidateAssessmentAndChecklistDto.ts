import { ICandidateAssessment } from "src/app/_models/hr/candidateAssessment";
import { IChecklistHRDto } from "./checklistHRDto";

export interface ICandidateAssessmentAndChecklistDto
{
     assessed: ICandidateAssessment,
     checklistHRDto: IChecklistHRDto
}