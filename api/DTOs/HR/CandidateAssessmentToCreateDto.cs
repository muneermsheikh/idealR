using System.ComponentModel.DataAnnotations;
using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CandidateAssessmentToCreateDto
    {
        public int CandidateId {get; set;}
        public int OrderItemId { get; set; }
        public bool RequireInternalReview {get; set;}
        [Required]
        public int ChecklistHRId {get; set;}
        public DateTime AssessedOn { get; set; }
        public string AssessedByEmployeeName {get; set;}
        public ICollection<CandidateAssessmentItem> CandidateAssessmentItems { get; set; }
    }
}