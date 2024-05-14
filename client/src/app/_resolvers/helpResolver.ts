import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, ResolveFn } from "@angular/router";
import { Observable, of } from "rxjs";
import { Help, IHelp } from "../shared/models/admin/help";


  export const CustomerReviewResolver: ResolveFn<IHelp | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('topic');
    if (id===null || id === '') return of(null);
    //return inject(HelpService).getCustomerReview(+id!);
    return new Help();

  };
  