import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IFeedbackHistoryDto } from "src/app/_dtos/admin/feedbackAndHistoryDto";
import { FeedbackService } from "src/app/_services/feedback.service";

export const FeedbackHistoryResolver: ResolveFn<IFeedbackHistoryDto[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var custid = route.paramMap.get('customerId') ?? '0';
    if (custid ==='0') return of(null);

    return inject(FeedbackService).getFeedbackHistory(+custid);
  };