export class OrderFwdParams
{
    orderNo=0;
    orderId=0;
    fwdDateFrom? = new Date();
    fwdDateUpto? = new Date();
    customerId: number = 0;
    professionId = 0;
    
    sort = "orderno";
    pageNumber = 1;
    pageSize = 10;
    search: string='';
}