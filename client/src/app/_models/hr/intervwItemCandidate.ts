import { IIntervwCandAttachment, IntervwCandAttachment } from "./intervwCandAttachment";
import { UserAttachment } from "./userAttachment";

export interface IIntervwItemCandidate {
     id: number;
     intervwItemId: number;
     candidateId: number;
     applicationNo: number;
     candidateName: string;
     passportNo: string;
     scheduledFrom: Date;
     reportedAt: Date;
     interviewedAt: Date;
     interviewStatus: string;
     interviewerRemarks: string;
     attachmentFileNameWithPath: string;
}

export class IntervwItemCandidate implements IIntervwItemCandidate {
     id= 0;
     intervwItemId= 0;
     candidateId= 0;
     applicationNo= 0;
     candidateName= '';
     passportNo= '';
     scheduledFrom= new Date;
     reportedAt=new Date;
     interviewedAt=new Date;
     interviewStatus = '';
     interviewerRemarks= '';
     attachmentFileNameWithPath = '';
}