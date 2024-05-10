namespace api.DTOs.Finance
{
    public class UpdatePaymentConfirmationDto
    {
        public int VoucherEntryId { get; set; }
        public bool DrEntryApproved { get; set; }
        public DateTime DrEntryApprovedOn { get; set; }
        public int DrEntryApprovedByEmployeeById {get; set;}
    }
}