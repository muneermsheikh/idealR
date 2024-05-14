import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProfession } from "src/app/_models/masters/profession";
import { CategoryService } from "src/app/_services/category.service";

 
export const CategoryListResolver: ResolveFn<IProfession[] | undefined> = (
  ) => {
      return inject(CategoryService).getCategoryList();
  };