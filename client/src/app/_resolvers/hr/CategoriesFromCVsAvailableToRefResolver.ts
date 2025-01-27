import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProspectiveHeaderDto } from "src/app/_dtos/hr/prospectiveHeaderDto";
import { CandidateAssessmentService } from "src/app/_services/hr/candidate-assessment.service";


export const CategoriesFromCVsAvailableToRefResolver: ResolveFn<IProspectiveHeaderDto[]|null> = (
  ) => {

    return inject(CandidateAssessmentService).categoriesOfCVsAvailableToRefer();
  };