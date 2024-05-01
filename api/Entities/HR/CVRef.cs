using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Process;

namespace api.Entities.HR
{
    public class CVRef: BaseEntity
    {
        public int CVReviewId { get; set; }
        [ForeignKey("OrderItemId")]
        public int OrderItemId { get; set; }
        [ForeignKey("CandidateId")]
        public int CandidateId { get; set; }
        public DateTime ReferredOn { get; set; } = DateTime.Now;
        public int HRExecId { get; set; }
        public string RefStatus { get; set; }="Referred";
        public DateTime RefStatusDate {get; set;}
        public ICollection<Candidate> Candidates {get; set;}
        public List<Deployment> Deployments {get; set;}
    }
}