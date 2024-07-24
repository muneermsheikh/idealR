import { IVoucherAttachment } from "./voucherAttachment";
import { IVoucherEntry, VoucherEntry } from "./voucherEntry";

export interface IVoucher
{
	partyName: string;
	id: number;
	divn: string;
	coaId: number;
	accountName: string;
	voucherNo: number;
	voucherDated: Date;
	username: string;
	reviewedByUsername: string;
	reviewedOn: Date;
	amount: number;
	narration: string;
	/*approved: boolean;
	approvedByEmployeeId: number;
	approvedOn: Date;
	*/
	voucherEntries: IVoucherEntry[];
	voucherAttachments: IVoucherAttachment[];
}

export class Voucher implements IVoucher
{
	partyName='';
	id=0;
	divn = '';
	coaId = 0;
	accountName='';
	voucherNo = 0;
	voucherDated = new Date();
	username = '';
	reviewedByUsername = '';
	reviewedOn=new Date();
	amount = 0;
	narration = '';
	/*approved= false;
	approvedByEmployeeId=0;
	approvedOn=new Date();
	*/
	voucherEntries: VoucherEntry[]=[];
	voucherAttachments: IVoucherAttachment[]=[];
}
