import { IOrderForwardCategoryOfficial } from "./dlForwardCategoryOfficial";


export interface IOrderForwardCategory
{
	checked: boolean;
	id: number;
	orderId: number;
	orderItemId: number;
	OrderForwardToAgentId: number;
	categoryId: number;
	categoryName: string;
	charges: number;
	OrderForwardCategoryOfficials: IOrderForwardCategoryOfficial[]
}

export class OrderForwardCategory implements IOrderForwardCategory
{
	checked= false;
	id= 0;
	orderId= 0;
	orderItemId= 0;
	OrderForwardToAgentId= 0;
	categoryId= 0;
	categoryName= '';
	charges= 0;
	OrderForwardCategoryOfficials: IOrderForwardCategoryOfficial[]=[];
}

//teachers: forwards, batches: category, students: officials
//teachersForm: forwardsForm