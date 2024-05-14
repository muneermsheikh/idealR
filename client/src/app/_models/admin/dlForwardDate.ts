export interface IDLForwardDate
{
	id: number;
	dlForwardItemId: number;
	orderItemId: number;
	customerOfficialId: number;
	dateTimeForwarded: Date;
	dateOnlyForwarded: Date;
	emailIdForwardedTo: string;
	phoneNoForwardedTo: string;
	whatsAppNoForwardedTo: string;
	loggedInEmployeeId: number;
}

export class DLForwardDate
{
	id: number=0;
	dlForwardItemId: number=0;
	orderItemId: number=0;
	customerOfficialId: number=0;
	dateTimeForwarded: Date=new Date();
	dateOnlyForwarded: Date = new Date();
	emailIdForwardedTo: string = '';
	phoneNoForwardedTo: string = '';
	whatsAppNoForwardedTo: string = '';
	loggedInEmployeeId: number = 0;
}