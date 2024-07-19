export interface IPendingDebitApprovalDto
{
	id: number;
	voucherEntryId: number;
	voucherNo: number;
	voucherDated: Date;
	drAccountId: number;
	drAccountName: string;
	drAmount: number;
	drEntryApproved: boolean;
	drEntryApprovedByUsername: string;
	drEntryApprovedOn: Date;
	selected: boolean;
}

export class PendingDebitApprovalDto implements IPendingDebitApprovalDto
{
	id=0;
	voucherEntryId = 0;
	voucherNo=0;
	voucherDated=new Date();
	drAccountId = 0;
	drAccountName = '';
	drAmount = 0;
	drEntryApproved = false;
	drEntryApprovedByUsername = '';
	drEntryApprovedOn = new Date();
	selected = false;
}