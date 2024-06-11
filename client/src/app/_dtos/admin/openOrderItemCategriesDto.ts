export interface IOpenOrderItemCategoriesDto
{
     checked: boolean;
     orderId: number;
     //customerId: number;
     orderNo: number;
     orderItemId: number;
     orderDate: Date;
     customerName: string;
     //professionId: number;
     categoryRefAndName: string;
     requireInternalReview: string;
     quantity: number;
}

export class OpenOrderItemCategoriesDto implements IOpenOrderItemCategoriesDto
{
     checked: boolean = false;
     orderNo = 0;
     orderId = 0;
     customerId = 0;
     orderDate = new Date('1900-01-01');
     customerName = '';
     orderItemId = 0;
     professionId: number = 0;
     categoryRefAndName = '';
     requireInternalReview = "N";
     quantity = 0;
}