namespace api.DTOs.Admin
{
    public class CallRecordBriefDto
    {
        public int Id {get; set;}
        public string Source {get; set;}
        public string CategoryRef { get; set; }
        public string CategoryName { get; set; }
        public string Gender {get; set;}
        public int CandidateId { get; set; }
        public string CandidateName {get; set;}
        
        public string UserType { get; set; }
        public string ResumeId { get; set; }  
        public string EmailId {get; set;}
        public string MobileNo { get; set; }
        public DateTime CreatedOn {get; set;}
        public string Status { get; set; }
    }
}