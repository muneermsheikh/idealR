using api.Entities.Master;

namespace api.Entities.Admin.Client
{
    public class CustomerIndustry: BaseEntity
    {
        public Customer Customer { get; set; }
        public Industry Industry { get; set; }
    }
}