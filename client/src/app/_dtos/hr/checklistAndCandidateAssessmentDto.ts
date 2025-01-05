import { ICandidateAssessment } from "src/app/_models/hr/candidateAssessment";
import { IChecklistHRDto } from "./checklistHRDto";
import { IChecklistHR } from "src/app/_models/hr/checklistHR";

export interface IChecklistAndCandidateAssessmentDto
{
     assessed: ICandidateAssessment,
     checklistHR: IChecklistHR
}