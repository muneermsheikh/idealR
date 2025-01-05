import { IIntervwCandAttachment, IntervwCandAttachment } from "./intervwCandAttachment";
import { UserAttachment } from "./userAttachment";

export interface IIntervwItemCandidate {
     id: number;
     intervwItemId: number;
     interviewItemId: number;
     candidateId: number;
     personId: string;
     prospectiveCandidateId: number;
     applicationNo: number;
     candidateName: string;
     passportNo: string;
     scheduledFrom: Date;
     reportedAt?: Date;
     interviewedAt?: Date;
     interviewStatus: string;
     interviewerRemarks: string;
     attachmentFileNameWithPath: string;
}

export class IntervwItemCandidate implements IIntervwItemCandidate {
     id= 0;
     interviewItemId= 0;
     intervwItemId: number=0;
     candidateId= 0;
     prospectiveCandidateId = 0;
     personId = '';
     applicationNo= 0;
     candidateName= '';
     passportNo= '';
     scheduledFrom= new Date;
     reportedAt?=new Date;
     interviewedAt?=new Date;
     interviewStatus = '';
     interviewerRemarks= '';
     attachmentFileNameWithPath = '';
}