using api.Entities.Finance;

namespace api.DTOs.Finance
{
    public class VoucherWithNewAttachmentDto
    {
        public FinanceVoucher Voucher { get; set; }
        public ICollection<VoucherAttachment> NewAttachments { get; set; }
    }
}