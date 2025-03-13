using System.Data.Common;
using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Deployments;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Deployments;
using api.Interfaces.Messages;
using api.Params.Deployments;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Deployment
{
    public class CandidateFlightRepository : ICandidateFlightRepository
    {
        private readonly DataContext _context;
        private ICollection<DeployStatus> _deployStatuses;
        private readonly IDeploymentRepository _depRep;
        private readonly IComposeMessagesAdminRepository _msgAdmn;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        public CandidateFlightRepository(DataContext context, IDeploymentRepository depRep, IConfiguration config,
            UserManager<AppUser> userManager, IComposeMessagesAdminRepository msgAdmn)
        {
            _config = config;
            _userManager = userManager;
            _msgAdmn = msgAdmn;
            _depRep = depRep;
            _context = context;
        }
        
        public async Task<string> DeleteCandidateFlight(int candidateFlightId)
        {
            var obj = await _context.CandidateFlights.FindAsync(candidateFlightId);
            if (obj == null) return "The object to delete does not exist";

            _context.Entry(obj).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to delete";
        }

        public async Task<string> EditCandidateFlight(CandidateFlight model )
        {
            var strErr="";

            var existing = await _context.CandidateFlights
                .AsNoTracking().FirstOrDefaultAsync();
            
            if(existing==null) return "The Object to delete does not exist";

            _context.Entry(existing).CurrentValues.SetValues(model);

            _context.Entry(existing).State=EntityState.Modified;
            
             try 
            {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strErr = "Database Error: " + ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            return strErr;
            
        }

        public async Task<ICollection<int>> GetCandidateFlightIdsCVRefIds(ICollection<int> cvRefIds)
        {
            var ids = await _context.CandidateFlights.Where(x => cvRefIds.Contains(x.CvRefId)).Select(x => x.Id).ToListAsync();
            return ids;
        }

        public async Task<CandidateFlightGrp> InsertCandidateFlights(CandidateFlightGrp flight)
        {
            _context.CandidateFlightGrps.Add(flight);
            
            return await _context.SaveChangesAsync() > 0 ? flight : null;
        }

        public async Task<DepPendingDtoWithErr> InsertDepItemsWithCandFlightItems(
            DepItemsWithCandFightGrpDto deps, AppUser user)
        {
            var dtowithErr = new DepPendingDtoWithErr();

            var flightGrp = await InsertCandidateFlights(deps.CandFlightToAdd);

            if(flightGrp==null) {
                dtowithErr.ErrorString = "failed to insert the candidate flight";
                return dtowithErr;
            }

            var depitems = new List<DepItem>();
            var sq = deps.DepItemsToAdd.Select(x => x.Sequence).FirstOrDefault();
            if(_deployStatuses == null || _deployStatuses.Count==0) _deployStatuses = await _depRep.GetDeploymentStatusData();
            
            var nextSeq = _deployStatuses.Where(x => x.Sequence == sq)
                .Select(x => new {nextSq=x.NextSequence, days=x.WorkingDaysReqdForNextStage}).FirstOrDefault();
            
            foreach(var item in deps.DepItemsToAdd) {
                var depitem = new DepItem{DepId=item.DepId, NextSequence=nextSeq.nextSq, Sequence=sq, 
                    NextSequenceDate=item.TransactionDate.AddDays(nextSeq.days), 
                    TransactionDate = item.TransactionDate};
                depitems.Add(depitem);
            }

            dtowithErr = await _depRep.AddDeploymentItems(depitems, user);  //return value is DepPendingBriefDto for use by client

            if(!string.IsNullOrEmpty(dtowithErr.ErrorString)) return dtowithErr;

            //update candidateflight with ddepitemid
            var modified=false;
            foreach(var flt in flightGrp.CandidateFlightItems) {
                var depitem = await _context.DepItems.Where(x => x.DepId==flt.DepId && x.Sequence==1300).FirstOrDefaultAsync();
                if(depitem != null) {
                    flt.DepItemId=depitem.Id;
                    _context.Entry(flt).State=EntityState.Modified;
                    modified=true; }

                if(modified) await _context.SaveChangesAsync();
            }
            return dtowithErr;
        }

        public async Task<PagedList<CandidateFlightGrp>> GetAllCandidatesFlightsGrp(CandidateFlightParams cParams)
        {
            var qry = _context.CandidateFlightGrps.OrderBy(x => x.DateOfFlight)
                .Include(x => x.CandidateFlightItems.OrderBy(m => m.ApplicationNo))
                .OrderByDescending(x => x.DateOfFlight)
                .AsQueryable();

            if(!string.IsNullOrEmpty(cParams.AirlineName)) qry = 
                qry.Where(x => x.AirlineName.ToLower()==cParams.AirlineName.ToLower());
            if(!string.IsNullOrEmpty(cParams.FlightNo)) 
                qry = qry.Where(x => x.FlightNo.ToLower()==cParams.FlightNo.ToLower());
            if(!string.IsNullOrEmpty(cParams.AirportOfBoarding)) 
                qry = qry.Where(x => x.AirportOfBoarding.ToLower()==cParams.AirportOfBoarding.ToLower());
            if(cParams.OrderNo!=0) qry = qry.Where(x => x.OrderNo==cParams.OrderNo);
            
             var paged = await PagedList<CandidateFlightGrp>.CreateAsync(qry.AsNoTracking()
                //.ProjectTo<CandidateFlightGrpDto>(_mapper.ConfigurationProvider)
                , cParams.PageNumber, cParams.PageSize);

            if(paged == null) return null;

            return paged;
        }
        public Task<string> EditCandidateFlight(CandidateFlightGrp candidateFlight)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<CandidateFlightItem>> GetCandidateFlightItems(int flightid)
        {
            var items = await _context.CandidateFlightItems.Where(x => x.CandidateFlightGrpId==flightid)
                .OrderBy(x => x.ApplicationNo)
                .ToListAsync();
            
            return items;
        }

        public async Task<string> GetOrGenerateTravelAdviseMessage(int flightid)
        {
            var msg = await _context.Messages.Where(x => x.FlightId==flightid).FirstOrDefaultAsync();
            if(msg != null) {
                return "Email message for that Flight Id " + flightid + 
                    " is already composed and available for viewing in Messages Section. " +
                    "Please look for Message dated " + string.Format("{0: dd-MMMM-yyyy}", msg.MessageComposedOn);
            }

            var flt = await _context.CandidateFlightGrps.Where(x => x.Id==flightid)
                .Include(x => x.CandidateFlightItems.OrderBy(n => n.ApplicationNo))
                .FirstOrDefaultAsync();

            if(flt== null) return  "No matching flight details on record";

            var order = await _context.Orders.Where(x => x.OrderNo==flt.OrderNo)
                .Include(x => x.Customer).ThenInclude(x => x.CustomerOfficials.Where(n => n.Status=="Active"))
                .FirstOrDefaultAsync();
            var official = new CustomerOfficial();

            official = order.Customer.CustomerOfficials.Where(x => x.PriorityAdmin==100).FirstOrDefault();
            if(official==null) {
                official = order.Customer.CustomerOfficials.Where(x => x.PriorityAccount==100).FirstOrDefault();
                official??=order.Customer.CustomerOfficials.Where(x => x.PriorityHR==100).FirstOrDefault();
            }
    
            var msgDate = string.Format("{0: dd-MMM-yyyy}", flt.DateOfFlight);
            
            var senderObj = await _userManager.FindByNameAsync(_config["AdminManagerAppUsername"] ?? "" ) 
                ?? await _userManager.FindByNameAsync(_config["DocControllerAdminAppUsername"] ?? "" );
            
            if(senderObj==null) return "Failed to retrieve Sender Object";

            var subject = "Travel Advise for " + flt.CandidateFlightItems.Count + " candidates";

            var subjectInBody = "<b><u>Subject: </b>Travel Advise for your " + flt.CandidateFlightItems.Count + 
                "candidates</u>";
      
            var msgBody = string.Format("{0: dd-MMM-yyyy}", msgDate) + "<br><br>" + 
                official.Title + " " + official.OfficialName + "<br>" + official.Designation +
                "<br>" + "M/S " + order.Customer.CustomerName +
                "<br>" + order.Customer.City + "<br><br>" +
                "Email: " + official.Email + "<br><br>" + 
                "copy: <br><br>Dear " + official.Gender=="M" ? "Sir: " : "Madam: ";
            msgBody += "<br><br>" + subjectInBody + "<br><br>";
            
            msgBody += "Pleased to advise your following candidates are travelling as per details provided:<br><br>";

            msgBody += "<b><u>Flight Particulars</u></b><br><Table border: 1px><Th width='100'>Date Of Flight</Th><Th width='100'>Boarding At</Th>";
            msgBody += "<Th width='100'>Destination</Th><Th width='75'>Airline</Th>";
            msgBody += "<Th width='75'>Flight No</Th><Th width='75'>ETD from <br>Boarding</Th><Th width='75'>ETA at<br>Destination</Th>";

            if(!string.IsNullOrEmpty(flt.AirportVia)) {
                msgBody +="<Th width='75'>Via Airport</Th><Th width='75'>ETA at " + flt.AirportVia + "</Th>";
                msgBody +="<Th width='75'>ETD from " + flt.AirportVia + "</Th>";
            }

            msgBody += "<Tr><Td>" + string.Format("{0: dd-MMM-yyyy}", flt.DateOfFlight) + "</Td>";
            msgBody += "<Td>" + flt.AirportOfBoarding + "</Td><Td>" + flt.AirportOfDestination + "</Td>";
            msgBody += "<Td>" + flt.AirlineName + "</Td><Td>" + flt.FlightNo + "</Td>";
            msgBody += "<Td>" + string.Format("{0: dd-MMM-yyyy HH:mm}", flt.ETD_Boarding) + "</Td>";
            msgBody += "<Td>" + string.Format("{0: dd-MMM-yyyy HH:mm}", flt.ETA_Destination) + "</Td>";

            if(!string.IsNullOrEmpty(flt.AirportVia)) {
                msgBody += "<Td>" + flt.AirportVia + "</Td>";
                msgBody += "<Td>" + string.Format("{0: dd-MMM-yyyy HH:mm}", flt.ETA_Via) + "</Td>";
                msgBody += "<Td>" + string.Format("{0: dd-MMM-yyyy HH:mm}", flt.ETD_Via) + "</Td>";
            }
 
            msgBody += "</Table><br><b><u>Candidates Travelling</b></u>";
            msgBody += "<br><Table border: 1px><Th width='35'</Th>";
            msgBody += "<Th width='150'>Category</Th><Th width='75'>Appl No</Th>";
            msgBody += "<Th width='150'>Candidate Name</Th>";
            int ct=0;
            foreach(var candidt in flt.CandidateFlightItems) {
                ct++;
                msgBody += "<Tr><Td text-align: 'right'>" + ct + "</Td><Td>" + candidt.CategoryName + "</Td>";
                msgBody += "<Td text-align: 'right'>" + candidt.ApplicationNo + "</Td><Td>" + candidt.CandidateName + "</Td>";
                msgBody += "<Td></Td>";
            }

            msgBody += "</Table><br><br>Kindly acknowledge receipt of this message and arrange to receive the candidates at ";
            msgBody += flt.AirportOfDestination + " airport.  Also, please provide name and contact details of the person receiving them at ";
            msgBody += "the airport.<br><br>Best Regards/<br>" +  _config["AdminManagerDesignation"] ?? "";
            msgBody += "<br font-size: 50%>TravelAdviseToClient FlightId=" + flightid;
            

            var message = new Message
            {
                SenderUsername=senderObj.UserName,
                //RecipientAppUserId= official.AppUserId,
                //SenderAppUserId=senderObj.Id,
                SenderEmail=senderObj.Email ?? "",
                RecipientUsername = official.UserName ?? "",
                RecipientEmail = official.Email ?? "",
                Subject = subject,
                Content = msgBody,
                MessageType = "TravelAdviseToClient",
                MessageComposedOn = flt.DateOfFlight,
                FlightId=flightid
            };

            _context.Messages.Add(message);

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to compose and save the message";

        }

    }
}