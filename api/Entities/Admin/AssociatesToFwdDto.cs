namespace api.Entities.Admin
{
    public class AssociatesToFwdDto
    {
        public int OfficialId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName {get; set;}
        public string CustomerCity { get; set; }
        public string Title {get; set;}
        public string CustomerOfficialName {get; set;}
        public string OfficialEmailId {get; set;}
        public string OfficialAppUserId {get; set;}
        public string Phoneno {get; set;}
        public string Mobile {get; set;}
        public string OfficialDesignation {get; set;}
        public bool Checked {get; set;}
        public bool CheckedPhone {get; set;}
        public bool CheckedMobile {get; set;}
    }
}