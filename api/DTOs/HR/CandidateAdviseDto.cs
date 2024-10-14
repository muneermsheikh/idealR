using api.Entities.HR;
using api.Entities.Identity;

namespace api.DTOs.HR
{
    public class CandidateAdviseDto
    {
		public AppUser RecipientObj { get; set; }				
		public AppUser SenderObj { get; set; }
		public int ApplicationNo { get; set; }
		public int CandidateId { get; set; }
		public int OrderItemId {get; set;}
		public int CVRefId { get; set; }
		public string CandidateTitle {get; set;}
		public string CandidateName {get; set;}
		public string CandidateGender { get; set; }
		public string CandidateEmail { get; set; }
		public string CandidateKnownAs {get; set;}
		public string CustomerName {get; set;}
		public string CustomerCity {get; set;}
		public int OrderNo { get; set; }
		public string SelectedAs { get; set; }
		
		//public int CandidateAppUserId { get; set; }
		public string HrExecEmail {get; set;}
		public string CandidateUsername {get; set;}
		public DateOnly TransactionDate { get; set; }
    }
}