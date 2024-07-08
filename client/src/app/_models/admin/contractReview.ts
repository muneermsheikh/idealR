import { IContractReviewItem } from "./contractReviewItem";

export interface IContractReview{
     id: number;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     customerName: string;
     reviewedBy?: number;
     reviewedOn: Date;
     hrExecAssignedToUsername: string;
     reviewStatus: string;          //enumReviewStatus
     medicalProcessInchargeUsername: string;
     visaProcessInchargeUsername: string;
     emigProcessInchargeUsername: string;
     travelProcessInchargeUsername: string;
     releasedForProduction: boolean;
     contractReviewItems: IContractReviewItem[];
}