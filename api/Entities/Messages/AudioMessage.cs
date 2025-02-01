using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using DocumentFormat.OpenXml.Wordprocessing;

namespace api.Entities.Messages
{
    public class AudioMessage: BaseEntity
    {
        public string RecipientUsername { get; set; }
        public string SenderUsername { get; set; }
        public string MessageText { get; set; }
        //public Blob MessageAudioFile { get; set; }
        [MaxLength(255)]
        public string FileName { get; set; }
        [MaxLength(100)]
        public string ContentType { get; set; }
        //public BinaryData Data { get; set; }
    }
}