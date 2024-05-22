export class OpenOrderItemsParams
{
    customerId: number = 0;
    orderId: number = 0;
    professionIds: number[]=[];
    orderItemIds: number[]=[];

    pageNumber = 1;
    pageSize = 10;
}