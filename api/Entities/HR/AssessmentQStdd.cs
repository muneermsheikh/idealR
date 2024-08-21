namespace api.Entities.HR
{
    public class AssessmentQStdd: BaseEntity
    {
        public int QuestionNo { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public bool IsMandatory { get; set; }
        public int MaxPoints { get; set; }
    }
}