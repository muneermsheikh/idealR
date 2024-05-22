export interface IVoucherToAddNewDto
{
	debitCOAId: number;
	debitAccountName: string;
	creditCOAId: number;
	creditAccountName: string;
	amount: number;
	voucherDate: Date;
	narration: string;
	drEntryRequiresApproval: boolean;
}

export class VoucherToAddNewDto implements IVoucherToAddNewDto
{
	debitCOAId = 0;
	debitAccountName = '';
	creditCOAId = 0;
	creditAccountName = '';
	amount= 0;
	voucherDate= new Date('1900-01-01');
	narration = '';
	drEntryRequiresApproval = false;
}