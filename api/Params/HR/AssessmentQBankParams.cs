namespace api.Params.HR
{
    public class AssessmentQBankParams: PaginationParams
    {
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
    }
}