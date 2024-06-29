using api.Entities.Admin;

namespace api.Entities.HR
{
    public class HRSkill: BaseEntity
    {
        public int EmployeeId { get; set; }
        public int ProfessionId { get; set; }      
        public string ProfessionName { get; set; }  
        public int IndustryId {get; set;}
        public int SkillLevel {get; set;}
        public bool IsMain {get; set;}
        public Employee Employee { get; set; }
    }
}