namespace api.Entities.Finance
{
    public class VoucherAttachment: BaseEntity
    {
        public VoucherAttachment()
        {
        }


        public int VoucherId { get; set; }
        public int AttachmentSizeInBytes { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public DateOnly DateUploaded { get; set; }
        public string UploadedByUsername { get; set; }
        public Voucher Voucher {get; set;}
    }
}