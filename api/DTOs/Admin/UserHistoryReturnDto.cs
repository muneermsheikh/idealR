using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class CallRecordReturnDto
    {
        public CallRecord CallRecord { get; set; }
        public string ErrorString { get; set; }
    }
}