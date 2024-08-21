using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Deployments;

namespace api.Entities.HR
{
    public class CVRef: BaseEntity
    {
        public int CandidateAssessmentId { get; set; }
        public int CVReviewId { get; set; }
        [ForeignKey("OrderItemId")]
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CandidateId")]
        public int CandidateId { get; set; }
        public DateTime ReferredOn { get; set; } = DateTime.Now;
        public int HRExecId { get; set; }
        public string HRExecUsername { get; set; }
        public string RefStatus { get; set; }="Referred";
        public string SelectionStatus { get; set; }
        public DateTime SelectionStatusDate { get; set; } 
        public DateTime RefStatusDate {get; set;}
        public ICollection<Candidate> Candidates {get; set;}
        public List<Dep> Deps {get; set;}
    }
}