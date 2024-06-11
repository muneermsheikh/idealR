namespace api.DTOs.Finance
{
    public class PendingDebitApprovalDto
    {
        public int VoucherItemId {get; set;}
        public int VoucherNo {get; set;}
        public DateTime VoucherDated {get; set;}
        public int DrAccountId { get; set; }
        public string DrAccountName { get; set; }
        public long DrAmount {get; set;}
        public bool DrEntryApproved {get; set;}
        public string DrEntryApprovedByAppUsername {get; set;}
        public DateTime DrEntryApprovedOn {get; set;}
    }
}