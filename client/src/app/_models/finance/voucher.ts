import { IVoucherAttachment } from "./voucherAttachment";
import { IVoucherItem, VoucherItem } from "./voucherItem";

export interface IVoucher
{
	id: number;
	divn: string;
	cOAId: number;
	accountName: string;
	voucherNo: number;
	voucherDated: Date;
	employeeId: number;
	reviewedById: number;
	reviewedByName: string;
	reviewedOn: Date;
	amount: number;
	narration: string;
	/*approved: boolean;
	approvedByEmployeeId: number;
	approvedOn: Date;
	*/
	voucherItems: IVoucherItem[];
	voucherAttachments: IVoucherAttachment[];
}

export class Voucher implements IVoucher
{
	id=0;
	divn = '';
	cOAId = 0;
	accountName='';
	voucherNo = 0;
	voucherDated = new Date();
	employeeId = 0;
	reviewedById = 0;
	reviewedByName = '';
	reviewedOn=new Date();
	amount = 0;
	narration = '';
	/*approved= false;
	approvedByEmployeeId=0;
	approvedOn=new Date();
	*/
	voucherItems: VoucherItem[]=[];
	voucherAttachments: IVoucherAttachment[]=[];
}
