export interface IOrderForwardCategoryOfficial
{
	checked: boolean;
	id: number;
	orderForwardCategoryId: number;
	customerOfficialId: number;
	officialName: string;
	agentName: string;
	dateForwarded: Date;
	//dateOnlyForwarded: Date;
	emailIdForwardedTo: string;
	phoneNoForwardedTo: string;
	whatsAppNoForwardedTo: String;
	username: string;
}

export class OrderForwardCategoryOfficial implements IOrderForwardCategoryOfficial
{
	checked = false;
	id= 0;
	orderForwardCategoryId= 0;
	agentName= '';
	customerOfficialId= 0;
	officialName = '';
	dateForwarded= new Date('1900-01-01');
	//dateOnlyForwarded= new Date('1900-01-01');
	emailIdForwardedTo= '';
	phoneNoForwardedTo= '';
	whatsAppNoForwardedTo= '';
	username= '';

	orderItemId= 0;
}