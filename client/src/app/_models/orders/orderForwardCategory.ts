import { IOrderForwardCategoryOfficial } from "./orderForwardCategoryOfficial";


export interface IOrderForwardCategory
{
	checked: boolean;
	id: number;
	orderId: number;
	orderNo: number;
	orderDate: Date;
	customerName: string;
	customerCity: string;

	orderItemId: number;
	professionId: number;
	professionName: string;
	charges: number;
	orderForwardCategoryOfficials: IOrderForwardCategoryOfficial[]
}

export class OrderForwardCategory implements IOrderForwardCategory
{
	checked= false;
	id= 0;
	orderId= 0;
	orderNo=0;
	orderDate=new Date();
	customerName='';
	customerCity='';
	
	orderItemId= 0;
	agentId= 0;
	professionId= 0;
	professionName= '';
	charges= 0;
	orderForwardCategoryOfficials: IOrderForwardCategoryOfficial[]=[];
}