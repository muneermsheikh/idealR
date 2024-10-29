import { IntervwItemCandidate } from "src/app/_models/hr/intervwItemCandidate";

export interface IInterviewAttendanceDto
{
    id: number;
    checked: boolean;
    customerName: string;
    orderNo: number;
    interviewId: number;
    orderItemId: number;
    interviewVenue: string;
    professionName: string;
    scheduledFrom: Date;
    interviewMode: string;
    reportedAt?: Date;
    interviewedAt?: Date;
    personId: string;
    applicationNo: number;
    candidateName: string;
    interviewStatus: string;
    interviewerRemarks: string;
}