import { IOrderForwardCategory } from "./orderForwardCategory";

export interface IOrderForwardToAgent
{
	id: number;
	orderId: number;
	orderNo: number;
	orderDate: Date;
	customerId: number;
	customerName: string;
	customerCity: string;
	projectManagerId: number;
	orderForwardCategories: IOrderForwardCategory[];
}

export class OrderForwardToAgent 
{
	id= 0;
	orderId= 0;
	orderNo= 0;
	orderDate: Date = new Date('1900-01-01');
	customerId= 0;
	customerName= '';
	customerCity= '';
	projectManagerId= 0;
	orderForwardCategories: IOrderForwardCategory[]=[];
}