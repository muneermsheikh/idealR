using System.ComponentModel.DataAnnotations;

namespace api.Entities.Finance
{
    public class VoucherItem: BaseEntity
    {
        public VoucherItem()
        {
        }

        public int VoucherId { get; set; }
        public DateOnly TransDate { get; set; }
        public int COAId { get; set; }
        public string AccountName { get; set; }
        public long Dr {get; set;}
        public long Cr {get; set;}
        public string Narration { get; set; }
        public string DrEntryApprovedByAppUsername { get; set; }
        public DateOnly? DrEntryApprovedOn { get; set; }
        public bool DrEntryApproved { get; set; }
        public string Remarks { get; set; }
    
    }
}