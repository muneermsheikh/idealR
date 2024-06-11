export interface IReviewItemQ
{
    id: number;
    orderItemId: number;
    contractReviewItemId: number;
    srNo: number;
    reviewParameter: string;
    response: boolean;
    responseText: string;
    isResponseBoolean: boolean;
    isMandatoryTrue: boolean;
    remarks: string;
}