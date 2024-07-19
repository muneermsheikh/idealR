

export interface IVoucherEntry{
	
	id: number;
	voucherId: number;
	transDate: Date;
	cOAId: number;
	dr: number;
	cr: number;
	accountName: string;
	narration: string;
	drEntryApproved: boolean;
	drEntryApprovedByUsername: string
	drEntryApprovedOn: Date;
}

export class VoucherEntry implements IVoucherEntry
{
	id=0;
	voucherId=0;
	transDate=new Date();
	cOAId=0;
	accountName = '';
	dr=0;
	cr=0;
	narration= '';
	requiresApproval=false;
	drEntryApproved = false;
	drEntryApprovedByUsername = '';
	drEntryApprovedOn = new Date();
}