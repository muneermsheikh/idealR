import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IProfession } from "src/app/_models/masters/profession";
import { CategoryService } from "src/app/_services/category.service";

export const CategoryResolver: ResolveFn<IProfession | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(CategoryService).getCategory(+id!);
  };