namespace api.Entities.Finance
{
    public class VoucherAttachment: BaseEntity
    {
        public VoucherAttachment()
        {
        }


        public int FinanceVoucherId { get; set; }
        public int AttachmentSizeInBytes { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public DateTime DateUploaded { get; set; }
        public string UploadedByUsername { get; set; }
        public FinanceVoucher Voucher {get; set;}
    }
}