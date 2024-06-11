export interface IOrderForwardToAgentDto
{
    id: number;
	orderNo: number;
	orderDate: Date;
	customerName: string;
	orderForwardCategories: IOrderForwardCategoryDto[];
}

export interface IOrderForwardCategoryDto
{
	id: number;
	orderForwardToAgentId: number;
	professionName: string;
	charges: number;
    orderForwardCategoryOfficials: IOrderForwardCategoryOffDto[];
}

export interface IOrderForwardCategoryOffDto
{
    id: number;
    orderForwardCategoryId: number;
    agentName: string;
	emailIdForwardedTo: string;
    dateTimeForwarded: Date;
}