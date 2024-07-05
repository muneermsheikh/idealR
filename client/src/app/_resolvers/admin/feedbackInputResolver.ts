import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IFeedbackInput } from "src/app/_models/hr/feedback";
import { FeedbackService } from "src/app/_services/feedback.service";

export const FeedbackInputResolver: ResolveFn<IFeedbackInput | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null || id === '0') return of(null);

    console.log('id in resolver', id);
    return inject(FeedbackService).getFeedbackObject(+id);
  };