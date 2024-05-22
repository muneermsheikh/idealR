import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ICustomerReviewData } from "../_models/admin/customerReviewData";
import { CustomerReviewService } from "../_services/admin/customer-review.service";

export const CustomerReviewStatusDataResolver: ResolveFn<ICustomerReviewData[]|null> = (
  ) => {
    return inject(CustomerReviewService).getCustomerReviewStatusData();
  };