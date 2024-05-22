export interface IContractReviewDto
{
    id: number;
    orderNo: number;
    orderDate: Date;
    customerName: string;
    reviewedByName: string;
    reviewedOn: Date;
    reviewStatus: string;
    releasedForProduction: boolean;
}