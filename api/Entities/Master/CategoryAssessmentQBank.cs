namespace api.Entities.Master
{
    public class CategoryAssessmentQBank: BaseEntity
    {
        public int AssessmentQBankId { get; set; }
        public string AssessmentParameter { get; set; }
        public int QNo { get; set; }
        public bool IsStandardQ { get; set; }
        public string Question { get; set; }
        public int MaxPoints { get; set; }
    }
}