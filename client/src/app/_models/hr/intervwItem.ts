import { IIntervwItemCandidate } from "./intervwItemCandidate";

export interface IIntervwItem {  
     id: number;
     intervwId: number;
     interviewVenue: string;
     orderItemId: number;
     professionId: number;
     professionName: string;
     interviewMode: string;
     interviewerName: string;
     estimatedMinsToInterviewEachCandidate: number;
     interviewItemCandidates: IIntervwItemCandidate[];
}
