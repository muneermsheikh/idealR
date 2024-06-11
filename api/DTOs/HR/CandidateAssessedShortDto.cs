using api.DTOs.Admin.Orders;

namespace api.DTOs.HR
{
    public class CandidateAssessedShortDto
    {
        public int CandidateAssessmentId { get; set; }
        public int OrderItemId { get; set; }
        public string CategoryRef { get; set; }
        public string CustomerName { get; set; }
        public int CandidateId {get; set;}
        public string CandidateName { get; set; }
        public DateTime AssessedOn { get; set; }
        public string AssessedByUsername {get; set;}
        public string RequireInternalReview {get; set;}
        public int ChecklistHRId {get; set;}
        public string AssessResult { get; set; } 
        public OrderItemBriefDto orderItemBriefDto {get; set;}
    }
}