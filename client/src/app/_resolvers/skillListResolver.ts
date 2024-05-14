import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { SharedService } from "../shared/services/shared.service";
import { ISkillData } from "../shared/models/hr/skillData";

export const SkillListResolver: ResolveFn<ISkillData[]> = (
    ) => {
          return inject(SharedService).getSkillData();
    };