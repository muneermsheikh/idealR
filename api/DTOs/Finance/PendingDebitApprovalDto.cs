namespace api.DTOs.Finance
{
    public class PendingDebitApprovalDto
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public int VoucherEntryId {get; set;}
        public int VoucherNo {get; set;}
        public DateOnly VoucherDated {get; set;}
        public int DrAccountId { get; set; }
        public string DrAccountName { get; set; }
        public long DrAmount {get; set;}
        public bool DrEntryApproved {get; set;}
        public string DrEntryApprovedByAppUsername {get; set;}
        public DateTime DrEntryApprovedOn {get; set;}
    }
}