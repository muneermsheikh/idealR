export interface IVoucherDto
{
	id: number;
	divn: string;
	accountName: string;
	voucherNo: number;
	voucherDated: Date;
	amount: number;
	narration: string;
	drEntryApproved: boolean;
	drEntryApprovedOn: Date;
	drEntryApprovedByUsername: string;
	loggedInName: string;
}