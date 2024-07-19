namespace api.DTOs.Customer
{
    public class OfficialAndCustomerNameDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OfficialName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public bool CustomerIsBlacklisted {get; set;}
    }
}