using System.Net.Http.Headers;
using api.Errors;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class OrdersExportController : BaseApiController
    {
        private readonly IOrdersRepository _orderRepo;
        public OrdersExportController(IOrdersRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        [HttpPost("orderexcelconversion"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> ConvertOrderExcelData()
        {
            var errString="";
            //check for uploaded files
            var files = Request.Form.Files;

            var folderName = Path.Combine("Assets", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if(files.Count > 0) 
            {
                try{
                    foreach (var file in files)
                    {
                        if(file.Length == 0) continue;

                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var FileExtn = Path.GetExtension(file.FileName);
                        if(FileExtn != ".xlsx") {
                            return "Invalid File Extension.  Only XLSX files allowed";
                        }

                        var filename=file.FileName[..9].ToLower();

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        errString = await _orderRepo.WriteOrdersExcelToDB(fullPath, User.GetUsername());

                        if(int.TryParse(errString, out _)) return Ok(errString);
                        
                        return BadRequest(new ApiException(400, errString, "Failed to copy the file to database" ) );
                    }
                
                } catch (Exception ex) {
                    return BadRequest(new ApiException(400, "Error in API", ex.Message));
                } 
            }
            
            if (!string.IsNullOrEmpty(errString)) throw new Exception("Failed to read and update the prospective file");

            return Ok("");
        }

  

    }
}