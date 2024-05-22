using api.Interfaces.HR;

namespace api.Data.Repositories.HR
{
    public class FileUploadRepository: IFileUploadRepository
    {
        private readonly DataContext _context;
        public FileUploadRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<string> GetFileUrl(int attachmentid) 
        {
               var attachment = await _context.UserAttachments.FindAsync(attachmentid);
               if (attachment==null) return "";

               var FileName=attachment.UploadedLocation + '/' + attachment.Name;
               if(string.IsNullOrEmpty(FileName)) {
                    FileName = Directory.GetCurrentDirectory() + "\\assets\\images\\" + attachment.Name;     //api is the current driectory
               }

               if(FileName.Contains('\\')) FileName = FileName.Replace(@"\\", @"\");

               if(!System.IO.File.Exists(@FileName)) return "";

               //var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

               return FileName;
          }

    }
}