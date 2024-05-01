
using api.Entities.Admin.Client;

namespace api.Entities.Master
{
    public class Industry: BaseEntity
    {
        public string IndustryName { get; set; }
        public ICollection<CustomerIndustry> customerIndustries { get; set; }   
        
    }
}