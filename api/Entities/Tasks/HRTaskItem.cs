using System.ComponentModel.DataAnnotations;

namespace api.Entities.Tasks
{
    public class HRTaskItem: BaseEntity
    {
        public int HRTaskId { get; set; }
        public DateTime TransactionDate { get; set; }
        [Required]
        public string HRExecutiveUsername { get; set; }
        [Required]
        public int ApplicationNo { get; set; }
        [Required]
        public int CandidateId { get; set; }
        public int CandidateAssessmentId { get; set; }
        [Required]
        public int CVRefId { get; set; }
        [MaxLength(150)]
        public string Remarks { get; set; }
    
    }
}