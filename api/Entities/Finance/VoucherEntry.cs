using System.ComponentModel.DataAnnotations;

namespace api.Entities.Finance
{
    public class VoucherEntry: BaseEntity
    {
     
        public VoucherEntry()
        {
        }

        public int FinanceVoucherId { get; set; }
        public DateOnly   TransDate { get; set; }
        public int COAId { get; set; }
        public string AccountName { get; set; }
        public long Dr {get; set;}
        public long Cr {get; set;}
        public string Narration { get; set; }
        public int DrEntryApprovedByEmployeeById { get; set; }
        [MaxLength(10)]
        public DateOnly DrEntryApprovedOn { get; set; }
        public bool DrEntryApproved { get; set; }
    
    }
}