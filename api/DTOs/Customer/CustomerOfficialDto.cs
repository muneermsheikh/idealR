namespace api.DTOs.Customer
{
    public class CustomerOfficialDto
    {
        public int OfficialId { get; set; }
        public string OfficialName { get; set; }
        public string Gender { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName {get; set;}
        public string AboutCompany {get; set;}
        public string CompanyKnownAs {get; set;}
        public string City {get; set;}
        public string Country {get; set;}
        public string Title { get; set; }
        public string Designation { get; set; }
        public string OfficialEmailId { get; set; }
        public int OfficialAppUserId {get; set;}
        public string MobileNo { get; set; }
        public bool Checked {get; set;}
        public bool CheckedPhone {get; set;}
        public bool CheckedMobile {get; set;}
        public bool CustomerIsBlacklisted { get; set; }
    }
}