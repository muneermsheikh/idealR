namespace api.DTOs.HR
{
    public class AssessmentQBankDto
    {
        public int Id { get; set; }
        public int ProfessionId { get; set; }   
        public string ProfessionName { get; set; }
        public int AssessmentQBankId { get; set; }
        public string AssessmentParameter { get; set; }
        public int QNo { get; set; }
        public bool IsStandardQ { get; set; }
        public bool IsMandatory { get; set; }
        public string Question { get; set; }
        public int MaxPoints { get; set; }
    }
}