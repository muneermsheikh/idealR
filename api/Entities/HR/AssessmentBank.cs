namespace api.Entities.HR
{
    public class AssessmentBank: BaseEntity
    {
        public int ProfessionId { get; set; }   
        public string ProfessionName { get; set; }
        public ICollection<AssessmentBankQ> AssessmentBankQs { get; set;}
    }

   
}