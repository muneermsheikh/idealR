import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IInterviewBrief } from "src/app/shared/models/hr/interviewBrief";
import { IPagination } from "src/app/shared/models/pagination";
import { InterviewService } from "src/app/shared/services/hr/interview.service";
 
export const InterviewsBriefResolver: ResolveFn<IPagination<IInterviewBrief[]> | null | undefined> = (
  ) => {
   return inject(InterviewService).getInterviews(false);
  };