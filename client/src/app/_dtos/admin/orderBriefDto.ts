export interface IOrderBriefDto
{
     id: number;
     orderNo: number;
     //orderId: number;
     orderDate: Date;
     //completeBy: Date;
     customerId: number;
     customerName: string;
     cityOfWorking: string;
     //cityOfWorking: string;
     status: string;
     //contractReviewStatus: string;
     contractReviewedOn?: Date;
     //contractReviewId: number;
     contractReviewStatus: string;
     //reviewedBy: number;
     //reviewedOn: Date;
     forwardedToHRDeptOn: Date;
     //orderItemsDto: IOrderItemDto[];
}

export interface IOrderItemDto{
     id: number;
     srNo: number;
     checked: boolean;
     orderId: number;
     orderItemId: number;
     categoryRef: string;
     categoryName: string;
     quantity: number;
     status: string;
}

export class OrderBriefDto implements IOrderBriefDto
{
     id = 0;
     orderNo = 0;
     orderDate = new Date();
     customerId = 0;
     customerName = '';
     cityOfWorking = '';
     status = '';
     contractReviewStatus = '';
     forwardedToHRDeptOn= new Date();
     contractReviewedOn?: Date = undefined;
     completeBy= new Date();
     //items: IOrderItemDto[]=[];

}