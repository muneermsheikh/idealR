using api.Entities.Admin;

namespace api.Entities.HR
{
    public class EmployeeOtherSkill: BaseEntity
    {
        public int EmployeeId { get; set; }
        public int SkillDataId { get; set; }
        public int SkillLevel { get; set; }
        public bool IsMain { get; set; }
        public Employee Employee { get; set; }
    }
}