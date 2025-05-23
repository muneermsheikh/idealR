namespace api.DTOs.Admin
{
    public class EmployeeBriefDto
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public string KnownAs { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Status { get; set; } = "Employed";
        public string City { get; set;} 
    }
}