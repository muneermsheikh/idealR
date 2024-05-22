
export interface IVoucherAttachment {
    id: number;
    voucherId: number;
    attachmentSizeInBytes: number;
    fileName: string;
    url: string;
    dateUploaded: Date;
    uploadedByUsername: string;
}

export class VoucherAttachment implements IVoucherAttachment {
    id: number=0;
    voucherId: number=0;
    attachmentSizeInBytes: number=0;
    fileName: string='';
    url: string='';
    dateUploaded: Date=new Date();
    uploadedByUsername="";
}