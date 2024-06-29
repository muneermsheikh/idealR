using api.Entities.HR;

namespace api.DTOs.Admin
{
    public class SelectionMessageDto
    {
        public string CustomerName {get; set;}
		public string CustomerCity {get; set;}
		public int OrderNo { get; set; }
		public string ProfessionName { get; set; }
        public string SelectionStatus { get; set; }
		//public string RejectionReason {get; set;}
	    public SelectionDecision selectionDecision {get; set;}
		public int ApplicationNo { get; set; }
		public int CandidateId { get; set; }
		public string CandidateTitle {get; set;}
		public string CandidateName {get; set;}
		public string CandidateGender { get; set; }
		public string CandidateEmail { get; set; }
		public string CandidateKnownAs {get; set;}
		public int HRSupAppUserId {get; set;}
		public int CandidateAppUserId { get; set; }
		public string HrExecUsername {get; set;}
		public string HRExecEmail {get; set;}
		public int HRSupId {get; set;}
		public Employment Employment {get; set;}
    }
}