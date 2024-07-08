export interface IOrderForwardToAgentDto
{
    id: number;
	orderId: number;
	orderNo: number;
	orderDate: Date;
	customerId: number;
	customerName: string;
	customerCity: string;
	orderForwardCategories: IOrderForwardCategoryDto[];
}

export interface IOrderForwardCategoryDto
{
	id: number;
	agentId: number;
	orderItemId: number;
	professionId: number;
	professionName: string;
	charges: number;
    orderForwardCategoryOfficials: IOrderForwardToOfficialDto[];
}

export interface IOrderForwardToOfficialDto
{
    id: number;
    orderForwardCategoryId: number;
	customerOfficialId: number;
	customerOfficialName: string;
    agentName: string;
	emailIdForwardedTo: string;
    dateForwarded: Date;
	phoneNoForwardedTo: string;
	whatsAppNoForwardedTo:string;
	username: string;
}