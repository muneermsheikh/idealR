import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IInterviewBrief } from "src/app/_models/hr/interviewBrief";
import { InterviewService } from "src/app/_services/hr/interview.service";
 
export const InterviewsBriefResolver: ResolveFn<IInterviewBrief[] | null | undefined> = (
  ) => {
   return inject(InterviewService).getInterviews(false);
  };