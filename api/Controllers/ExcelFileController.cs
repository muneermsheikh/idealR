using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class ExcelController : BaseApiController
    {
        public ExcelController()
        {
        }

        [HttpPost("prospectiveorapplications"), DisableRequestSizeLimit]
        public  ActionResult ConvertProspectiveData()
        {
            //check for uploaded files
            var files = Request.Form.Files;

            string ErrorString="";
       
            if(files.Count > 0) 
            {
                try{
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    foreach (var file in files)
                    {
                        if(file.Length == 0) continue;

                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var FileExtn = Path.GetExtension(file.FileName);
                        if(FileExtn != ".xlsx") continue;

                        var filename=file.FileName[..9].ToLower();

                        if(filename != "prospecti" && filename !="applicati") continue;

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) continue;

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        /*if(filename=="prospecti") {
                            ErrorString = await _excelService.ReadAndSaveProspectiveXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        } else {
                            ErrorString = await _excelService.ReadAndSaveApplicationsXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        }
                        */

                    }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok();
        }

        [HttpPost("customers"), DisableRequestSizeLimit]
        public  ActionResult ConvertCustomerData()
        {
            //check for uploaded files
            var files = Request.Form.Files;

            string ErrorString="";
       
            if(files.Count > 0) 
            {
                try{
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    foreach (var file in files)
                    {
                        if(file.Length == 0) continue;

                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var FileExtn = Path.GetExtension(file.FileName);
                        if(FileExtn != ".xlsx") continue;

                        var filename=file.FileName[..8].ToLower();

                        if(filename != "customer") continue;

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) continue;

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        /*if(filename=="prospecti") {
                            ErrorString = await _excelService.ReadAndSaveProspectiveXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        } else {
                            ErrorString = await _excelService.ReadAndSaveApplicationsXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        }
                        */

                    }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok();
        }

    }
}