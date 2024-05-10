namespace api.Params.Admin
{
    public class EmployeeParams: PaginationParams
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public string Email {get; set;}
        public string Department { get; set; }
        public string Status { get; set; } = "Employed";
    }
}