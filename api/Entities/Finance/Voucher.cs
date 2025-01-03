using System.ComponentModel.DataAnnotations;

namespace api.Entities.Finance
{
    public class VoucherTODELETE: BaseEntity
    {
        public VoucherTODELETE()
        {
        }

        //public string PartyName { get; set; }
        public int CVRefId { get; set; }
        [MaxLength(1)]
        public string Divn {get; set;}
        public int COAId { get; set; }
        public string AccountName { get; set; }
        [MaxLength(10), Required]
        public int VoucherNo { get; set; }
        public DateTime VoucherDated {get; set;}
        public long Amount {get; set;}
        public string Narration {get; set;}
        [Required]
        public ICollection<VoucherItem> VoucherItems { get; set; }
        public ICollection<VoucherAttachment> VoucherAttachments { get; set; }
        
    }
}