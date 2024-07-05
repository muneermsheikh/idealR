namespace api.DTOs.Admin
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email {get; set;}
        public string CustomerStatus {get; set;}
      
    }
}