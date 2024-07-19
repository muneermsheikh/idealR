export interface IVoucherToAddNewPaymentDto
{
	partyName: string;
	debitCOAId: number;
	debitAccountName: string;
	creditCOAId: number;
	creditAccountName: string;
	amount: number;
	paymentDate: Date;
	narration: string;
	drEntryRequiresApproval: boolean;
}

export class VoucherToAddNewPaymentDto implements IVoucherToAddNewPaymentDto
{
	partyName='';
	debitCOAId = 0;
	debitAccountName = '';
	creditCOAId = 0;
	creditAccountName = '';
	amount= 0;
	paymentDate= new Date('1900-01-01');
	narration = '';
	drEntryRequiresApproval = false;
}