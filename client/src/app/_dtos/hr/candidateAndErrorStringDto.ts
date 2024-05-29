import { ICandidate } from "src/app/_models/hr/candidate";

export interface ICandidateAndErrorStringDto
{
    candidate: ICandidate;
    errorString: string;
}