using System.ComponentModel.DataAnnotations;

namespace api.Entities.Finance
{
    public class FinanceVoucher: BaseEntity
    {
        public string PartyName { get; set; }
        [MaxLength(1)]
        public string Divn {get; set;}
        public int COAId { get; set; }
        public string AccountName { get; set; }
        [MaxLength(10), Required]
        public int VoucherNo { get; set; }
        public DateOnly VoucherDated {get; set;}
        public long Amount {get; set;}
        public string Narration {get; set;}
        [Required]
        public ICollection<VoucherEntry> VoucherEntries { get; set; }
        public ICollection<VoucherAttachment> VoucherAttachments { get; set; }  
        
        
    }
}