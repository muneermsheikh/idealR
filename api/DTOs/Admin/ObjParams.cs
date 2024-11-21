using api.Params;

namespace api.DTOs.Admin
{
    public class ObjParams: PaginationParams
    {
        public DateTime Fromdate { get; set; }
        public DateTime Uptodate { get; set; }
        public string AssignedToUsername { get; set; }
        public int OrderItemId { get; set; }
    }
}