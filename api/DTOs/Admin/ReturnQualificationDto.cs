using api.Entities.Master;

namespace api.DTOs.Admin
{
    public class ReturnQualificationDto
    {
        public Qualification qualification { get; set; } 
        public string ErrorString { get; set; }
    }
}