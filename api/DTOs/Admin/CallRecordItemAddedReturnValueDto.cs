using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class CallRecordItemAddedReturnValueDto
    {
        public string ContactResult { get; set; }
        public DateTime DateOfContact { get; set; }
        public bool MessageComposed { get; set; }
        public string ErrorString { get; set; }
    }
}