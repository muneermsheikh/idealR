namespace api.DTOs.HR
{
    public class 
    CandidateAssessmentDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string CustomerName { get; set; }    
        public int OrderItemId { get; set; }
        public string CategoryRef { get; set; }
        public int OrderId { get; set; }
        public string ProfessionName { get; set; }
        public string  RequireInternalReview { get; set; }
        public string AssessedByUsername { get; set; }
        public string AssessResult { get; set; }
        public DateTime AssessedOn { get; set; }
        public int ChecklistHRId { get; set; }  
        public string ChecklistedByName { get; set; }   
        public DateTime ChecklistedOn { get; set; }
        public ICollection<AssessmentItemDto> AssessmentItemsDto {get; set;}
    }

    public class AssessmentItemDto
    {
        public int Id { get; set; }
        public int CandidateAssessmentId { get; set; }
        public int QuestionNo { get; set; }
        public string Question { get; set; }
        public bool IsMandatory { get; set; }   
        public int MaxPoints { get; set; }
        public int Points { get; set; }
        public string Remarks { get; set; }
    }
}