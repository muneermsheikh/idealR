using api.Entities.Admin.Client;

namespace api.DTOs.Admin
{
    public class CustomerAndOfficialsDto
    {
        public int CustomerId { get; set; }
        public int OfficialId {get; set;}
        public int AppUserId {get; set;}
        public string CustomerName { get; set; }
        public string City {get; set;}
        public string Country {get; set;}
        public string OfficialName { get; set; }
        public string Designation {get; set;}
        public string OfficialEmailId { get; set; }
        public string MobileNo { get; set; }
        public string OfficialTitle { get; set; }
       
        
    }
}
