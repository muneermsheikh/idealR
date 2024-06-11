using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Deployments;


namespace api.Entities.HR
{
    public class CVRef: BaseEntity
    {
        public int CandidateAssessmentId { get; set; }
        [ForeignKey("OrderItemId")]
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CandidateId")]
        public int CandidateId { get; set; }
        public DateTime ReferredOn { get; set; } 
        public string HRExecUsername { get; set; }
        public string RefStatus { get; set; }// =Referred
        public DateOnly RefStatusDate {get; set;}
        public string SelectionStatus {get; set;}
        public DateTime SelectionStatusDate {get; set;}
        public Process Process { get; set; }
        public ICollection<Candidate> Candidates {get; set;}

    }
}