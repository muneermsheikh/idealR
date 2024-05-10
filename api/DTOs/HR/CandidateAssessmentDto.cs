namespace api.DTOs.HR
{
    public class CandidateAssessmentDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; } = false;
        public int OrderItemId { get; set; }
        public int CandidateId {get; set;}
        public DateTime AssessedOn { get; set; }
        public string AssessedByEmployeeName {get; set;}
        public bool RequireInternalReview {get; set;}
        public int ChecklistHRId {get; set;}
        public string AssessResult { get; set; } = "Not Assessed";
    }
}