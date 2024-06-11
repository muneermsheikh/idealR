namespace api.Entities.Admin.Order
{
    public class OrderForwardToOfficialDto
    {
        public int OrderForwardCategoryId { get; set; }
        public string AgentName {get; set;}
        public int CustomerOfficialId { get; set; }
        public string CustomerOfficialName { get; set; }
        public DateTime DateForwarded {get; set;}
        public string EmailIdForwardedTo { get; set; }
        public string PhoneNoForwardedTo { get; set; }
        public string WhatsAppNoForwardedTo { get; set; }
        public string Username {get; set;}
    }
}