export interface ICustomerReviewItem{
     id: number;
     customerReviewId: number;
     transactionDate: Date;
     username: string;
     customerReviewStatus: string;
     remarks: string;
     approvedByUsername: string;
     approvedOn: Date;
}