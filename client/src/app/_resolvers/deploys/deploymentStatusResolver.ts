import { inject } from "@angular/core";
import { IDeployStage } from "../../shared/models/masters/deployStage";
import { ResolveFn } from "@angular/router";
import { DeployService } from "../../shared/services/deploy.service";

export const DeploymentStatusResolver: ResolveFn<IDeployStage[]|null> = (
  ) => {
    return inject(DeployService).getDeployStatus();
  };