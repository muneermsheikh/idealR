namespace api.Entities.HR
{
    public class AssessmentQBank: BaseEntity
    {
        public int ProfessionId { get; set; }   
        public string ProfessionName { get; set; }
        public ICollection<AssessmentStddQ> AssessmentStddQs { get; set; }
     
    }
}