import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ICandidateBriefDto } from "src/app/_dtos/admin/candidateBriefDto";
import { CandidateService } from "src/app/_services/candidate.service";

export const CandidateBriefResolver: ResolveFn<ICandidateBriefDto[]> = (
  ) => {
    return inject(CandidateService).getCandidates(false);
  };