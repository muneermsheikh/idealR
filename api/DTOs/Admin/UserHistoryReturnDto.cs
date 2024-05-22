using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class UserHistoryReturnDto
    {
        public UserHistory UserHistory { get; set; }
        public string ErrorString { get; set; }
    }
}