namespace api.DTOs.Admin
{
    public class OfficialIdsAndOrderItemIdsDto
    {
        public ICollection<int> OfficialIds { get; set; }
        public ICollection<int> OrderItemIds { get; set; }
    }
}