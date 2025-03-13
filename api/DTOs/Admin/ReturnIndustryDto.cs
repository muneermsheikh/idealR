using api.Entities.Master;

namespace api.DTOs.Admin
{
    public class ReturnIndustryDto
    {
        public Industry industry { get; set; }
        public string ErrorString { get; set; }
    }
}