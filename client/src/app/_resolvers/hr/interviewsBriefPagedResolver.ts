import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IInterviewBrief } from "src/app/_models/hr/interviewBrief";
import { PaginatedResult } from "src/app/_models/pagination";
import { interviewParams } from "src/app/_models/params/Admin/interviewParams";
import { InterviewService } from "src/app/_services/hr/interview.service";
 
export const InterviewsBriefPagedResolver: ResolveFn<PaginatedResult<IInterviewBrief[] | null | undefined>> = (
  ) => {
    var service = inject(InterviewService);
    var params = new interviewParams();
  
    service.setParams(params);
    return service.getInterviewsPaged(false);

  };