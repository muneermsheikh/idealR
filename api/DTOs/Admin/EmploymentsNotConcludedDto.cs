namespace api.DTOs.Admin
{
    public class EmploymentsNotConcludedDto
    {
        public int Id { get; set; }    
        public string CandidateName {get; set;}
        public string ProfessionName {get; set;}
        public string CompanyName { get; set; }
        public string SelectionStatus { get; set; }
        public DateOnly SelectedOn { get; set; }
        public int Charges {get; set;}
        public string SalaryCurrency { get; set; }
        public int Salary { get; set; }
    
    }
}