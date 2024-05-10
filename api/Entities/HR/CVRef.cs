using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Process;

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
        public DateOnly ReferredOn { get; set; } 
        public int HRExecId { get; set; }
        public string RefStatus { get; set; }="Referred";
        public DateOnly RefStatusDate {get; set;}
        public string SelectionStatus {get; set;}
        public DateOnly SelectionStatusDate {get; set;}
        public ICollection<Candidate> Candidates {get; set;}
        public List<Deployment> Deployments {get; set;}
    }
}