import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProspectiveHeaderDto } from "src/app/_dtos/hr/prospectiveHeaderDto";
import { ProspectiveService } from "src/app/_services/hr/prospective.service";

 export const ProspectiveHeaderResolver: ResolveFn<IProspectiveHeaderDto[]|null> = (
  ) => {
    
    return inject(ProspectiveService).getProspectiveHeadersDto("decline");
  };