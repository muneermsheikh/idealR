using api.Entities.HR;

namespace api.DTOs.HR
{
    public class ChecklistObj
    {
        public ChecklistObj()
        {
        }


        public ChecklistHR  ChecklistHR { get; set; }
        public string ErrorString { get; set; }
    }

    public class CVRefObject
    {
        public CVRefObject()
        {
        }

        public ICollection<MessageDto>  MessageDtos { get; set; }
        public string ErrorString { get; set; }
    }

}