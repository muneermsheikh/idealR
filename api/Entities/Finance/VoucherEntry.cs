using System.ComponentModel.DataAnnotations;

namespace api.Entities.Finance
{
    public class VoucherEntry: BaseEntity
    {
     
        public VoucherEntry()
        {
        }

        public int FinanceVoucherId { get; set; }
        public DateTime TransDate { get; set; }
        public int CoaId { get; set; }
        public string AccountName { get; set; }
        public long Dr {get; set;}
        public long Cr {get; set;}
        public string Narration { get; set; }
        public string DrEntryApprovedByUsername { get; set; }
        public DateTime? DrEntryApprovedOn { get; set; }
        public bool? DrEntryApproved { get; set; }
        public string Remarks { get; set; }
    
    }
}