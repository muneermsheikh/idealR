using DocumentFormat.OpenXml.Office2010.Excel;

namespace api.DTOs.Finance
{
    public class VoucherToAddNewDto
    {
        public int DebitCOAId { get; set; }
        public string DebitAccountName { get; set; }
        public Id CreditCOAId { get; set; }
        public string CreditAccountName { get; set; }
        public long Amount { get; set; }
        public DateOnly VoucherDate { get; set; }
        public string Narration { get; set; }
        public bool DrEntryRequiresApproval { get; set; }
    }
}