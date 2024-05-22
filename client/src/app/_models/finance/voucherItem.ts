

export interface IVoucherItem{
	id: number;
	financeVoucherId: number;
	transDate: Date;
	coaId: number;
	dr: number;
	cr: number;
	accountName: string;
	narration: string;
	drEntryApproved: boolean;
	drEntryApprovedByEmployeeId: number;
	drEntryApprovedOn: Date;
}

export class VoucherItem implements IVoucherItem
{
	id=0;
	financeVoucherId=0;
	transDate=new Date();
	coaId=0;
	accountName = '';
	dr=0;
	cr=0;
	narration= '';
	requiresApproval=false;
	drEntryApproved = false;
	drEntryApprovedByEmployeeId = 0;
	drEntryApprovedOn = new Date();
}