namespace api.DTOs.Admin
{
    public class CVFwdMsgDto
    {
        public CVFwdMsgDto()
        {
        }

        public int CandidateAssessmentId {get; set;}
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public int OfficialId { get; set; }
        public int OfficialAppUserId {get; set;}
        public string OfficialTitle { get; set; }
        public string OfficialName { get; set; }
        public int OfficialUserId { get; set; }
        public string Designation { get; set; }
        public string OfficialEmail { get; set; }
        public int OrderItemId {get; set;}
        public int OrderNo { get; set; }
        public DateOnly OrderDated { get; set; }
        public int ItemSrNo { get; set; }
        public string CategoryName { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string PPNo { get; set; }
        public int CumulativeSentSoFar { get; set; }
        public string AssessmentGrade { get; set; }
    }
}