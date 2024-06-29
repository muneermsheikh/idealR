using api.Entities.HR;

namespace api.DTOs.Admin
{
    public class EmployeeIdAndKnownAsDto
    {
        public int Id { get; set; }
        public string KnownAs { get; set; }
        public string Username {get; set;}
        public ICollection<HRSkill> HrSkills {get; set;}
    }
}