using api.Entities.HR;

namespace api.DTOs.Admin
{
    public class AppUserBriefDto
    {
        public int AppUserId { get; set; }
        public string KnownAs { get; set; }
        public string Username {get; set;}
        public string Name { get; set;}
        public string AppUserEmail { get; set;}
        public string Position { get; set;}
    }
}