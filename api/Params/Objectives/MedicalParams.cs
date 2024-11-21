namespace api.Params.Objectives
{
    public class MedicalParams: PaginationParams
    {
        public DateTime FromDate { get; set; }
        public DateTime UptoDate { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeUsername { get; set; }    
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }

    }
}
