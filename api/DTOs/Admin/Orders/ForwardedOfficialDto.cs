namespace api.DTOs.Admin.Orders
{
    public class ForwardedOfficialDto
    {
        public int ForwardCategoryId { get; set; }
        public string OfficialName {get; set;}
        public string AgentName {get; set;}
        public DateOnly DateOnlyForwarded {get; set;}
        public string EmailIdForwardedTo { get; set; }
        public string PhoneNoForwardedTo { get; set; }
        public string WhatsAppNoForwardedTo { get; set; }
        public string Username {get; set;}
    }
}