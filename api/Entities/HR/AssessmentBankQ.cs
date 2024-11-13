namespace api.Entities.HR
{
    public class AssessmentBankQ: BaseEntity
    {
        public int AssessmentBankId { get; set; }
        public string AssessmentParameter { get; set; }
        public int QNo { get; set; }
        public bool IsStandardQ { get; set; }
        public bool IsMandatory { get; set; }
        public string Question { get; set; }
        public int MaxPoints { get; set; }
    }
}