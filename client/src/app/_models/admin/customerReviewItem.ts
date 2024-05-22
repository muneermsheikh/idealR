export interface ICustomerReviewItem{
     id: number;
     customerReviewId: number;
     reviewTransactionDate: Date;
     username: string;
     customerReviewDataId: number;
     remarks: string;
     approvedBySupUsername: boolean;
     approvedOn: Date;
}