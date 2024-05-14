import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IPagination } from "../shared/models/pagination";
import { InterviewService } from "../shared/services/hr/interview.service";
import { IInterviewBrief } from "../shared/models/hr/interviewBrief";

 export const CustomerReviewResolver: ResolveFn<IPagination<IInterviewBrief[]> | null | undefined> = (
  ) => {
     return inject(InterviewService).getInterviews(false);
  };