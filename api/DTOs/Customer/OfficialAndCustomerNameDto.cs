namespace api.DTOs.Customer
{
    public class OfficialAndCustomerNameDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OfficialName { get; set; }
    }
}