

using api.Entities.Admin;

namespace api.DTOs.HR
{
    public class InterviewItemWithErrDto
    {
        public IntervwItem intervwItem { get; set; }
        public string Error { get; set; }
    }
}