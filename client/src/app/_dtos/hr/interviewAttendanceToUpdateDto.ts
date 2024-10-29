export interface IInterviewAttendanceToUpdateDto
{
    interviewCandidateId: number;
    reportedAt: Date;
    interviewedAt: Date;
    interviewStatus: string;
    interviewStatusDate: Date;
    interviewRemarks: string;
}

export class InterviewAttendanceToUpdateDto implements IInterviewAttendanceToUpdateDto
{
    interviewCandidateId=0;
    reportedAt = new Date;
    interviewedAt = new Date;
    interviewStatus = '';
    interviewStatusDate = new Date;
    interviewRemarks = '';
}