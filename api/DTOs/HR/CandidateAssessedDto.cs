namespace api.DTOs.HR
{
    public class CandidateAssessedDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; } = false;
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string ProfessionName { get; set; }
        public int ApplicationNo { get; set; }
        public string CustomerName { get; set; }
        public int CandidateId {get; set;}
        public string CandidateName { get; set; }
        public DateOnly AssessedOn { get; set; }
        public string AssessedByUsername {get; set;}
        public string RequireInternalReview {get; set;}
        public int ChecklistHRId {get; set;}
        public string AssessResult { get; set; } = "Not Assessed";
    }
}