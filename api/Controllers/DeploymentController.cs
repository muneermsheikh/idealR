using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.Admin;
using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Deployments;
using api.Entities.Messages;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Deployments;
using api.Params.Deployments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "ProcessPolicy")]       //RequireRole("Document Controller-Processing", "Processing Manager", "Process Supervisor", "Admin"));
    public class DeploymentController : BaseApiController
    {
        private readonly IDeploymentRepository _depRepo;
        private readonly ICandidateFlightRepository _flightRepo;
        public DeploymentController(IDeploymentRepository depRepo, ICandidateFlightRepository flightRepo)
        {
            _flightRepo = flightRepo;
            _depRepo = depRepo;
        }

        [HttpGet("bycvrefid/{cvrefid}")]
        public async Task<ActionResult<Dep>> GetDeploymentByCVRefId (int cvrefid)
        {
            var data = await _depRepo.GetDeploymentByCVRefId(cvrefid);
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    
        [HttpGet("deployStatusData")]
        public async Task<ActionResult<DeployStatus>> GetDeploymentStatusData ()
        {
            var data = await _depRepo.GetDeploymentStatusData();
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    
        [HttpGet("deployStatusSeqAndNames")]
        public async Task<ActionResult<DeployStatusAndName>> GetDeploymentStatusSeqAndNames ()
        {
            var data = await _depRepo.GetDeploymentSeqAndStatus();
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    

        [HttpGet("deployments")]
        public async Task<ActionResult<PagedList<DeploymentPendingDto>>> GetDeployments ([FromQuery]DeployParams depParams)
        {
            var data = await _depRepo.GetDeployments(depParams);
            
            if(data == null || data.Count ==0) return NotFound(new ApiException(400, "No data found", ""));

              Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, data.PageSize, 
                data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }

        [HttpGet("deployment/{id}")]
        public async Task<ActionResult<Dep>> GetDeploymentWithItems(int id)
        {
            var dep = await _depRepo.GetDeploymentByDepId(id);

            if(dep==null) return NotFound(new ApiException(400, "Not Found", "Failed to get the deployment record"));

            return Ok(dep);
        }
    
        [HttpPost("depItems")]
        public async Task<ActionResult<ICollection<DeploymentPendingBriefDto>>> AddDeploymentItems(ICollection<DepItemToAddDto> depItemsDto)
        {
            var depitems = new List<DepItem>();
            foreach(var item in depItemsDto) {
                var depitem = new DepItem{DepId=item.DepId, TransactionDate=item.TransactionDate, Sequence=item.Sequence};
                depitems.Add(depitem);
            }
            var dtos = await _depRepo.AddDeploymentItems(depitems, User.GetUsername());
            if(!string.IsNullOrEmpty(dtos.ErrorString)) return BadRequest(new ApiException(400, "Failed to add the deployment item", dtos.ErrorString));

            return Ok(dtos.DeploymentPendingBriefDtos);
        }
    
        [HttpPut("deployment")]
        public async Task<ActionResult<DeploymentPendingBriefDto>> EditDeployment(Dep dep)
        {
            var dto = await _depRepo.EditDeployment(dep);
            if(!string.IsNullOrEmpty(dto.ErrorString)) return BadRequest(new ApiException(400, "Failed to edit deployment", dto.ErrorString));

            return Ok(dto.DeploymentPendingBriefDtos.FirstOrDefault());
        }

        [HttpPut("depItem")]
        public async Task<ActionResult<bool>> EditDeploymentItem(DepItem depItem)
        {
            var strErr  = await _depRepo.EditDepItem(depItem);

            if(!string.IsNullOrEmpty(strErr)) 
                return BadRequest(new ApiException(400, "Failed to update the deployment item", strErr));

            return Ok("the deployment item was successfully updated");
        }

        /*[HttpPost("insertItemsWithFlight")]
        public async Task<ActionResult<ICollection<DeploymentPendingBriefDto>>> InsertDepItemsWithFlights(ICollection<DepItemWithFlightDto> depItemsWithFlight)
        {
            var dto = await _depRepo.InsertDepItemsWithFlights(depItemsWithFlight, User.GetUsername());

            if(!string.IsNullOrEmpty(dto.ErrorString)) return BadRequest(new ApiException(400, "Failed to insert the dept items", dto.ErrorString));

           return Ok(dto.DeploymentPendingBriefDtos);
 
        }*/
        
        [HttpDelete("depItem/{depItemId}")]
        public async Task<ActionResult<bool>> DeleteDeploymentItem(int depItemId)
        {
            var strErr = await _depRepo.DeleteDepItem(depItemId);
            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400, "Failed to delete the dep Item", strErr));

            return Ok("the deployment item was deleted successfully");
        }

        
        [HttpDelete("dep/{depId}")]
        public async Task<ActionResult<bool>> DeleteDeployment(int depId)
        {
            var strErr = await _depRepo.DeleteDep(depId);
            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400, "Failed to delete the dep Item", strErr));

            return Ok("the deployment record was deleted successfully");
        }

        [HttpGet("nextDepItemFromCVRef/{cvrefid}")]
        public async Task<ActionResult<int>> GetNextDepItemToAddFomCVRefId(int cvrefid)
        {
            var depItem = await _depRepo.GetNextDepItemToAddFomCVRefId(cvrefid);

            if(depItem==null) return Ok("Failed to generate the Deployment Item object");

            return Ok(depItem);
        }

       /* [HttpGet("flightdata")]
        public async Task<ActionResult<FlightDetail>> GetFlightData()
        {
            var data = await _depRepo.GetFlightData();
            if(data ==null) return NotFound(new ApiException(400, "Not Found", "No flight data available to retrieve"));
            return Ok(data);
        } */

        //candidate flights
        [HttpPut("candidateflightids")]
        public async Task<ActionResult<ICollection<int>>> GetFlightIds(ICollection<int> cvrefids) {
            var ids = await _flightRepo.GetCandidateFlightIdsCVRefIds(cvrefids);

            return Ok(ids);
        }

     
        [HttpDelete("CandidateFlight/{candidateFlightId}")]
        public async Task<ActionResult<string>> DeleteCandidateFlight(int candidateFlightId)
        {
            var deleted = await _flightRepo.DeleteCandidateFlight(candidateFlightId);

            if(deleted=="") return BadRequest(new ApiException(400, "Failure", "Failed to delete the Candidate Flight"));

            return Ok("");
        }

        [HttpGet("CandidateFlights")]
        public async Task<ActionResult<PagedList<CandidateFlightGrp>>> GetCandidateFlightsGrp([FromQuery]CandidateFlightParams cParams)
        {
            var pagedList = await _flightRepo.GetAllCandidatesFlightsGrp(cParams);

            if(pagedList.Count ==0) return BadRequest("failed to retrieve matching orders");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }

        
      
        [HttpPut("CandidateFlight")]
        public async Task<ActionResult<string>> EditCandidateFlight(CandidateFlightGrp candidateFlight)
        {
            var updated = await _flightRepo.EditCandidateFlight(candidateFlight);
            return updated;
        }

        [HttpPost("depItemsAndCandFlight")]
        public async Task<ActionResult<ICollection<DeploymentPendingBriefDto>>> AddDepItemsWithCandFlightHeaders(DepItemsWithCandFightGrpDto dto)
        {
            var err="";
            if (DateTime.TryParse(dto.CandFlightToAdd.ETA_DestinationString, out DateTime dt)) {
                dto.CandFlightToAdd.ETA_Destination = dt;} else {err="Invalid ETA Destination time";}
            
            if (DateTime.TryParse(dto.CandFlightToAdd.ETD_BoardingString, out DateTime dt2)) {
                dto.CandFlightToAdd.ETD_Boarding = dt2;}else {err +=" Invalid ETA Arrival Time";}
            
            if (DateTime.TryParse(dto.CandFlightToAdd.ETA_ViaString, out DateTime dt3)) {
                dto.CandFlightToAdd.ETA_Via = dt3;}else {err +=" Invalid Via ETD Arrival Time";}
            
            if (DateTime.TryParse(dto.CandFlightToAdd.ETD_ViaString, out DateTime dt4)) {
                dto.CandFlightToAdd.ETD_Via = dt4;}else {err +=" Invalid Via ETD Departure Time";}
            
            if(!string.IsNullOrEmpty(err)) return BadRequest(new ApiException(400, "Invalid Date - ", err));

            //dto.CandFlightToAdd.ETD_Boarding = DateTime.Parse(dto.CandFlightToAdd.ETD_BoardingString);
            
            var dtoToReturn = await _flightRepo.InsertDepItemsWithCandFlightItems(dto, User.GetUsername());

            if(!string.IsNullOrEmpty(dtoToReturn.ErrorString)) return BadRequest(new ApiException(400, "Failed to insert dep items and candidate flights", dtoToReturn.ErrorString));

            return Ok(dtoToReturn.DeploymentPendingBriefDtos);
        }

        [HttpGet("CandidateFlightItems/{id}")]
        public async Task<ActionResult<ICollection<CandidateFlightItem>>> GetCandidateFlightItems(int id)
        {
            var dtoToReturn = await _flightRepo.GetCandidateFlightItems(id);

            if(dtoToReturn.Count==0) return BadRequest(new ApiException(400, "Not Found", "No candidates found in that Flight"));

            return Ok(dtoToReturn);
        }

        [HttpGet("traveladvise/{flightid}")]
        public async Task<ActionResult<Message>> GetOrGenerateTravelAdvise(int flightid)
        {
            var err = await _flightRepo.GetOrGenerateTravelAdviseMessage(flightid);

            if(!string.IsNullOrEmpty(err)) return BadRequest(new ApiException(400, "Bad Request", err));

            return Ok("Message composed and saved in Messages!");
        }

        
         [HttpPut("insertItemsWithFlightAttachment")]
        public async Task<ActionResult<string>> InsertDepItemsWithFlightAttachments()
        {
            var folderName = Path.Combine("Assets", "FlightTickets");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            try
            {
                var depItemsData = JsonSerializer.Deserialize<ICollection<DepItem>>(Request.Form["data"],  
                        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                var depCandidateFlightsData = JsonSerializer.Deserialize<CandidateFlightGrp>(Request.Form["candidateFlights"],  
                        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;
               
                var memoryStream = new MemoryStream();

                var file=files[0];
                var fullPath="";
                if (file.Length > 0) {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    fullPath = Path.Combine(pathToSave, fileName);        //physical path
                    if(System.IO.File.Exists(fullPath)) {
                        return BadRequest(new ApiException(400, "File already exists", "The file already exists at location " 
                            + pathToSave + ". Please choose another file or delete the file at the existing location"));
                    }

                    var dbPath = Path.Combine(folderName, fileName); 

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);
                    //modelData.OfferAttachmentFullPath=fullPath;
                }
                
                var errDto = await _depRepo.AddDeploymentItems(depItemsData,User.GetUsername());

                depCandidateFlightsData.FullPath=fullPath;
                
                var strErr = await _flightRepo.InsertCandidateFlights(depCandidateFlightsData);
                
                return string.IsNullOrEmpty(errDto.ErrorString) 
                    ?  Ok("") 
                    : BadRequest(new ApiException(400, "Error in upload", errDto.ErrorString));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }

        }

        [HttpGet("Housekeeping")]
        public async Task<ReturnStringsDto> Housekeeping()
        {
            var dto = await _depRepo.DoHousekeepingOfDeployments();

            return dto;
        }
       
    }
}