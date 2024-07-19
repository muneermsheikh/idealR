import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IFeedback } from "src/app/_models/hr/feedback";
import { FeedbackService } from "src/app/_services/feedback.service";

export const FeedbackInputResolver: ResolveFn<IFeedback | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id') ?? '0';
    var custid = route.paramMap.get('customerId') ?? '0';
    if (id === '0' && custid ==='0') return of(null);

    return inject(FeedbackService).getFeedbackObject(+id, +custid);
  };