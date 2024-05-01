namespace api.Entities.HR
{
    public class CandidateAssessmentItem: BaseEntity
    {
        public int CandidateAssessmentId { get; set; }
        public int QuestionNo { get; set; }
        public string AssessmentGroup { get; set; }
        public string Question { get; set; }
        public bool IsMandatory { get; set; }
        public bool AssessedOnTheParameter {get; set;}
        public int MaxPoints { get; set; }
        public int Points { get; set; }     //points given to the candidate
        
        public string Remarks { get; set; }
    }
}