import { IIntervwItemCandidate } from "./intervwItemCandidate";

export interface IIntervwItem {  
     id: number;
     intervwId: number;
     interviewVenue: string;
     venueAddress: string;
     venueAddress2: string;
     venueCityAndPIN: string;
     siteRepName: string;
     sitePhoneNo: string;
     orderNo: number;
     orderItemId: number;
     professionId: number;
     professionName: string;
     interviewMode: string;
     interviewerName: string;
     estimatedMinsToInterviewEachCandidate: number;
     interviewItemCandidates: IIntervwItemCandidate[];
}
