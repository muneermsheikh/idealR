namespace api.Params.HR
{
    public class CallRecordHeaderParams: PaginationParams
    {
        public int? Id { get; set; }
        public string CategoryRefCode { get; set; }
        public string CustomerName { get; set; }
        public string AssignedToUsername { get; set; }
        public string AssignedByUsername { get; set; }
        public string Status { get; set; }
        public bool Concluded {get; set;}
    }
}