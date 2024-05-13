namespace api.Params.Deployments
{
    public class DeployParams: PaginationParams
    {
        public ICollection<int> CVRefIds { get; set; }
        public ICollection<int> OrderItemIds { get; set; }
        public ICollection<int> CandidateIds {get; set;}
        public DateOnly SelectedOn {get; set;}
        public int OrderNo {get; set;}
        public int CustomerId { get; set; }
        public string CurrentStatus {get; set;}
    }
}