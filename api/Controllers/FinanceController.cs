using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.Finance;
using api.Entities.Finance;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Finance;
using api.Params.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace api.Controllers
{
    [Authorize(Policy = "AccountsPolicy")]
    public class FinanceController : BaseApiController
    {
        private readonly IFinanceRepository _finRepo;
        private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);
        public FinanceController(IFinanceRepository finRepo)
        {
            _finRepo = finRepo;
        }

        [HttpGet("DrApprovalsPending")]
        public async Task<ActionResult<PagedList<PendingDebitApprovalDto>>> GetPendingDebitApproval(DrApprovalParams pParams)
        {
            var data = await _finRepo.GetPendingDebitApprovals(pParams);

            if(data == null || data.Count ==0) return NotFound(new ApiException(400, "No data found", ""));

              Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, data.PageSize, 
                data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }

        [HttpGet("coalist")]
        public async Task<ActionResult<ICollection<COA>>> GetCOAList(COAParams coaParams)
        {
            var obj = await _finRepo.GetCOAList(coaParams);

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "no matching Chart of ACcount"));

            return Ok(obj);
        }

        [HttpGet("coapagedlist")]
        public async Task<ActionResult<PagedList<COA>>> GetCOAPagedList(COAParams coaParams)
        {
            var pagedList = await _finRepo.GetCOAPagedList(coaParams);
            if(pagedList.Count ==0) return BadRequest("No order items on record matching the criteria");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }

        [HttpGet("debitapprovals")]
        public async Task<ActionResult<PagedList<PendingDebitApprovalDto>>> GetDebitApprovalsPending(DrApprovalParams drParams)
        {
            var data = await _finRepo.GetPendingDebitApprovals(drParams);
            if(data == null || data.Count ==0) return NotFound(new ApiException(400, "No data found", ""));

              Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, data.PageSize, 
                data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }
       
        [HttpPost("candidateCOA/{applicationno}/{create}")]
        public async Task<ActionResult<COA>> GetOrCreateCandidateCOA(int applicationno, bool create)
        {
            var coa = await _finRepo.GetOrCreateCoaForCandidateWithNoSave(applicationno, create);
            if(coa==null) return BadRequest(new ApiException(400,"Bad Request", "Failed to create Chart of account for the candidate"));

            return Ok(coa);
        }

        [HttpPost("coa")]
        public async Task<ActionResult<COA>> AddNewCOA(COA coa)
        {
            var obj = await _finRepo.SaveNewCOA(coa);

            if(obj==null) return BadRequest(new ApiException(400, "Bad Request", "Failed to save the new COA"));

            return Ok(obj);

        }
        
        [HttpPut("coa")]
        public async Task<ActionResult<COA>> EditCOA(COA coa)
        {
            return await _finRepo.EditCOA(coa);
        }

        [HttpDelete("coa/{id}")]
        public async Task<ActionResult<bool>> DeleteCOA(int id)
        {
            var obj = await _finRepo.DeleteCOA(id);

            if(!obj) return BadRequest(new ApiException(400, "Bad Request", "Failed to delete the COA"));

            return Ok(obj);
        }

        [HttpGet("matchingcoas/{coaname}")]
        public async Task<ActionResult<ICollection<COA>>> GetMatchingCOAs(string coaname)
        {
            var obj = await _finRepo.GetMatchingCOANames(coaname);

            return Ok(obj);
        }

        //vouchers
        [HttpGet("nextvoucherno")]
        public async Task<ActionResult<int>> GetNextVoucherNo()
        {
            return await _finRepo.GetNextVoucherNo();
        }
        [HttpGet("voucherspagedlist")]
        public async Task<ActionResult<PagedList<Voucher>>> GetVouchersPagedList(VoucherParams vParams)
        {
            var pagedList = await _finRepo.GetVouchers(vParams);
            if(pagedList.Count ==0) return BadRequest("No order items on record matching the criteria");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }

        [HttpPut("updatePaymentConfirmation")]
        public async Task<ActionResult<bool>> UpdatePaymentConfirmation(ICollection<UpdatePaymentConfirmationDto> pDtos)
        {
            var updated = await _finRepo.UpdateCashAndBankDebitApprovals(pDtos);
            if(!updated) return BadRequest(new ApiException(400, "Bad Request","Failed to update the payment confirmations"));
            
            return Ok(updated);
        }

        [HttpPut("voucher")]
        public async Task<ActionResult<Voucher>> EditVoucher(Voucher voucher)
        {
            var existing = await _finRepo.EditVoucher(voucher);
            if(!existing) return BadRequest(new ApiException(400, "Bad Request", "Failed to update the Voucher"));

            return Ok("Voucher updated");
        }

        [HttpDelete("deletevoucher/{id}")]
        public async Task<ActionResult<bool>> DeleteVoucher(int id)
        {
            var deleted = await _finRepo.DeleteVoucher(id);
            if(!deleted) return BadRequest(new ApiException(400, "Bad Request", "Failed to delete the Voucher"));

            return Ok("Voucher deleted");
        }

        [HttpPost("newvoucher")]
        public async Task<ActionResult<Voucher>> AddNewVoucher(Voucher voucher)
        {
            var newvoucher = await _finRepo.AddNewVoucher(voucher, User.GetUsername());

            if(newvoucher==null) return BadRequest(new ApiException(400, "Bad Request", "failed to add the new voucher"));

            return Ok(newvoucher);

        }

        [HttpPost("newvoucherwithattachment"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> AddNewVoucherWithAttachment(Voucher voucher)
        {
            List<VoucherAttachment> Attachmentlist = new();
               

               try
               {
                    var modelData = JsonSerializer.Deserialize<Voucher>(Request.Form["data"],   //THE Voucher OBJECT
                         new JsonSerializerOptions {
                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                         });
                    
                    var files = Request.Form.Files;
                    //create the Voucher object first, because the voucherAttaachments need Voucher.VoucherEntry.Id
                    
                    var voucherAdded= await _finRepo.AddNewVoucher(modelData, User.GetUsername());

                    if(voucherAdded==null) {
                        
                        return BadRequest(new ApiException(400, "Bad request", "Failedd to create the voucher"));
                    }
                    
                    if(files==null || files.Count==0) return Ok("");

                    //save the uploaded file stream in the designated folder
                    var voucherId = voucherAdded.VoucherNo;
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    var memoryStream = new MemoryStream();

                    foreach (var file in files)
                    {
                         if (file.Length==0) continue;
                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         
                         var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                         if(System.IO.File.Exists(fullPath)) continue;
                         var dbPath = Path.Combine(folderName, fileName); 

                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                            file.CopyTo(stream);
                         }
                        //insert in voucherAttachments
                        var attachment = new VoucherAttachment{
                            VoucherId=voucherId,
                            AttachmentSizeInBytes = Convert.ToInt32(file.Length/1024), 
                            FileName=file.FileName, 
                            Url=fullPath, 
                            DateUploaded=_today, 
                            UploadedByUsername=User.GetUsername()
                        };
                        Attachmentlist.Add(attachment);
                    }

                    if (Attachmentlist.Count > 0) await _finRepo.AddVoucherAttachments(Attachmentlist);
                    return "";
               }
               catch (Exception ex)
               {
                    return  BadRequest(new ApiException(500, "Internal server error" + ex.Message));
               }

        }
    
        [HttpPut("updateVoucherwithattachment")]
         public async Task<ActionResult> UpdateVoucherWithUpload()
          {
              int voucherid=0;

               var voucherAttachmentlist = new List<VoucherAttachment>();

               try
               {
                    var modelData = JsonSerializer.Deserialize<Voucher>(Request.Form["data"],  
                         new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                    
                    var files = Request.Form.Files;                    
                    var voucherObjectDto = await _finRepo.UpdateFinanceVoucherWithFileUploads(modelData);
                    
                    var voucherObject = voucherObjectDto.Voucher;
                    var newAttacments = voucherObjectDto.NewAttachments;  //not written to file

                    if(voucherObject==null) return BadRequest(new ApiException(404, "Failed to update Finance Voucher object"));
                    voucherid=voucherObject.Id;
                    
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                    //var attachmentTypes = modelData.UserAttachments;

                    foreach (var file in files)     //files are new ones uploaded
                    {
                         if(file.Length == 0) continue;
                         /* 
                         1. files uploaded but not present in existing file attachments are the ones to be uploaded, 
                            and hence also to be added in _context.VoucherAttachments Object
                         2. The voucherAttachments collection  could already be having files uploaded earlier, and physical fils in the images folder, 
                            those are to be ignored and not added to the _context.UserAttachments object
                         */
                         //if(modelData.VoucherAttachments.Any(x => x.FileName == file.FileName)) continue;   //no need to upload files that are NOT NEW

                         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                         if(System.IO.File.Exists(pathToSave + @"\" + fileName)) continue;
                         
                         var fullPath = Path.Combine(pathToSave, fileName);
                         var dbPath = Path.Combine(folderName, fileName);
                         
                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                              file.CopyTo(stream);
                         }

                         newAttacments.Add(new VoucherAttachment{
                             VoucherId=modelData.Id, 
                             AttachmentSizeInBytes=Convert.ToInt32(file.Length/204),
                             Url=fullPath, 
                             FileName =@"'\" + folderName + @"\" + fileName + "'",
                             DateUploaded=_today,
                            UploadedByUsername = User.GetUsername()                              
                         }); 

                    }

                    if(newAttacments.Count > 0) await _finRepo.AddVoucherAttachments(newAttacments);
    
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(new ApiException(500, "Internal server error", ex.Message));
               }
          }
  
        [HttpGet("soa/{coaid}/{datefrom}/{dateupto}")]
        public async Task<ActionResult<StatementOfAccountDto>> GetStatementOfAccount(int coaid, DateOnly datefrom, DateOnly dateupto)
        {
            var obj = await _finRepo.GetStatementOfAccount(coaid, datefrom, dateupto);
            if(obj==null) return BadRequest(new ApiException(500, "Server Error", "failed to get the statement of account based on criteira given"));

            return Ok(obj);
        }
    }
}