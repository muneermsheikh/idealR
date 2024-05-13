namespace api.Params.Admin
{
    public class EmploymentParams: PaginationParams
    {
        public List<int> CVRefIds { get; set; }
        public List<int> SelectionDecisionIds { get; set; }
        public DateOnly SelectedOn { get; set; }
    }
}