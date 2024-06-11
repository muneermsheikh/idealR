export interface IOrderItemIdAndHRExecEmpNoDto
{
    orderItemId: number;
    hrExecEmpId: number;
    hrExecUsername: string;
    hrExecAppUserId: number;
}

export class OrderitemidAndHRExecDetails implements IOrderItemIdAndHRExecEmpNoDto
{
    orderItemId=0;
    hrExecEmpId=0;
    hrExecUsername='';
    hrExecAppUserId=0;
}