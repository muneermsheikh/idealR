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
     status: string;
     contractReviewedOn?: Date;
     contractReviewId: number;
     contractReviewStatus: string;
     acknowledgedToClientOn: Date;
     forwardedToHRDeptOn: Date;
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
     contractReviewId=0;
     forwardedToHRDeptOn= new Date();
     contractReviewedOn?: Date = undefined;
     completeBy= new Date();
     acknowledgedToClientOn=new Date();
     //items: IOrderItemDto[]=[];

}