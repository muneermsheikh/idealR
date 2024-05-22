import { IVoucher } from "src/app/_models/finance/voucher";
import { IVoucherAttachment } from "src/app/_models/finance/voucherAttachment";

export interface IVoucherWithNewAttachmentDto
{
    voucher: IVoucher;
    newAttachments: IVoucherAttachment[];
}