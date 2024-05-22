import { IOrderItemDto } from "../admin/orderBriefDto";

export interface IOrderToCreateDto
{
    orderDate: Date;
    orderRef: string;
    orderRefDate: Date;
    customerId: number;
    country: string;
    cityOfWorking: string;
    completeBy: Date;
    remarks: string;
    orderItems: IOrderItemDto[];
}