import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IDeployStage, IDeployStatusAndName } from "src/app/_models/masters/deployStage";
import { DeployService } from "src/app/_services/deploy.service";

export const DeployStatusAndSeqResolver: ResolveFn<IDeployStatusAndName[]|null> = (
  ) => {
    return inject(DeployService).getDepStatusAndNames();
  };