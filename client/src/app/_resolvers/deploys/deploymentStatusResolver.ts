import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IDeployStage } from "src/app/_models/masters/deployStage";
import { IDeploymentStatus } from "src/app/_models/masters/deployStatus";
import { DeployService } from "src/app/_services/deploy.service";

export const DeploymentStatusResolver: ResolveFn<IDeploymentStatus[]|null> = (
  ) => {
    return inject(DeployService).getDeployStatuses();
  };