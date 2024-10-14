using api.Entities.HR;

namespace api.DTOs.Admin
{
    public class AppUserReturnDto
    {
        public int AppUserId { get; set; }
        public string Username {get; set;}
        public string Error { get; set; }
    }
}