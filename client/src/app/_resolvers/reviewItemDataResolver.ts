import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IReviewItemData } from "../shared/models/admin/reviewItemData";
import { ReviewService } from "../shared/services/admin/review.service";

export const ReviewItemDataResolver: ResolveFn<IReviewItemData[] > = (
    ) => {
          return inject(ReviewService).getReviewData();
    };