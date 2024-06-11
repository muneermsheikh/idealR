import { IOrderForwardCategoryOfficial, OrderForwardCategoryOfficial } from "./orderForwardCategoryOfficial";

export interface IOrderForwardCategory
{
	checked: boolean;
	id: number;
	orderId: number;
	orderItemId: number;
	orderForwardToAgentId: number;
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
	orderItemId= 0;
	orderForwardToAgentId= 0;
	professionId= 0;
	professionName= '';
	charges= 0;
	orderForwardCategoryOfficials: OrderForwardCategoryOfficial[]=[];
}