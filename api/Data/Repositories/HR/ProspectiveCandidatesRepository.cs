using api.DTOs;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Helpers;
using api.HR.DTOs;
using api.Interfaces;
using api.Interfaces.HR;
using api.Params.Admin;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing.Controls;

namespace api.Data.Repositories.HR
{
    public class ProspectiveCandidatesRepository : IProspectiveCandidatesRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;
        public ProspectiveCandidatesRepository(DataContext context, ITokenService tokenService, 
            IConfiguration config, IUserRepository userRepo,
            IMapper mapper,UserManager<AppUser> userManager, ICandidateRepository candidateRepository)
        {
            _userRepo = userRepo;
            _config = config;
            _mapper = mapper;
            _tokenService = tokenService;
            _candidateRepository = candidateRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ProspectiveReturnDto> ConvertProspectiveToCandidate(int prospectiveId, string KnownAs, string EmailId, string username)
        {
            var prospective = await _context.ProspectiveCandidates.FindAsync(prospectiveId);

            if(prospective == null || string.IsNullOrEmpty(prospective.Email)) {
                //check if candidaate record exists
                var candidt = await _context.Candidates
                    .Where(x => x.Email.ToLower() == EmailId.ToLower() && x.KnownAs.ToLower()==KnownAs.ToLower())
                    .Select(x => new{ x.FullName, x.Id, x.AppUserId, x.Username, x.ApplicationNo, x.Gender} )
                    .FirstOrDefaultAsync();

                if(candidt != null) {
                    if(!string.IsNullOrEmpty(candidt.Username)) {
                        var appusr = await _userManager.FindByNameAsync(candidt.Username) 
                            ?? await _userManager.FindByEmailAsync(EmailId) ?? await _userManager.FindByIdAsync(candidt.AppUserId.ToString());
                        if(appusr != null) {
                            var dtoRet = new ProspectiveReturnDto{CandidateId=candidt.Id, 
                                ApplicationNo=candidt.ApplicationNo, CandidateUsername=appusr.UserName, ProspectiveCandidateId=prospectiveId};
                            return dtoRet;
                        } else {
                            appusr = new AppUser {KnownAs = KnownAs, Gender = candidt.Gender, Email = EmailId,UserName = EmailId};
                            var result = await _userManager.CreateAsync(appusr, "newPassword0#");
                            if (!result.Succeeded) return null;
                            await _userManager.AddToRoleAsync(appusr, "Candidate");
                            var dtoRet = new ProspectiveReturnDto{CandidateId=candidt.Id, ApplicationNo=candidt.ApplicationNo, CandidateUsername=appusr.UserName, ProspectiveCandidateId=prospectiveId};
                            return dtoRet;
                        }
                    }
                }
                return null;
            }
            
            //prospective is not null
            var user = await _userManager.FindByEmailAsync(prospective.Email);
            var charPosition= !prospective.CandidateName.Contains(' ') ? prospective.CandidateName.IndexOf("-", StringComparison.Ordinal):0;

            var prospectiveKnownAs = charPosition <=0 ? prospective.CandidateName : prospective.CandidateName[..charPosition];
            
            if(user == null) {     
                user = new AppUser
                { KnownAs = prospectiveKnownAs, Gender = prospective.Gender, PhoneNumber = prospective.PhoneNo,
                        Email = prospective.Email, UserName = prospective.Email};
                var result = await _userManager.CreateAsync(user, "newPassword0#");
                if (!result.Succeeded) return null;
                var roleResult = await _userManager.AddToRoleAsync(user, "Candidate");
            }

            //prepare to convert prospective to candidate               
            var userphones = new List<UserPhone>{new() {MobileNo=prospective.PhoneNo, IsMain=true, IsValid=true}};
            if(!string.IsNullOrEmpty(prospective.AlternateNumber)) {
                var userph=new UserPhone{MobileNo=prospective.AlternateNumber, IsMain=false, IsValid=true};
                userphones.Add(userph);
            }

            var userprofessions = new List<UserProfession>{new() {ProfessionId=prospective.ProfessionId}};
            var cvDto = new RegisterDto{
                Gender="M", FirstName = prospective.CandidateName, KnownAs = prospectiveKnownAs, 
                Username = prospective.Email, Email = prospective.Email, AppUserId = user.Id,
                ReferredByName = prospective.Source, UserProfessions=userprofessions, UserPhones=userphones,
                City = prospective.CurrentLocation, Nationality = prospective.Nationality
            };


            if (!string.IsNullOrEmpty(prospective.Age)) {
                var age = prospective.Age[..2];
                cvDto.DOB = DateTime.Today.AddYears(-Convert.ToInt32(age));
            }
          
            // finally, create the object candidate
            var cand = await _candidateRepository.CreateCandidateAsync(cvDto, username);

            if (cand == null) return null;
            
            //once succeeded, delete the record from prospective list.
            _context.ProspectiveCandidates.Remove(prospective);
            _context.Entry(prospective).State=EntityState.Deleted;

            await _context.SaveChangesAsync();
            var dto = new ProspectiveReturnDto {CandidateId = cand.Id, ApplicationNo = cand.ApplicationNo,
                ProspectiveCandidateId = prospectiveId, CandidateUsername = user.UserName };

            return dto;
        }

        //called by Interview.UpdateIntervwItem while adding new candidates to IntervwItemCandidates.  
        //this does not delete from Prospective after converting to Candidates, because the calling program
        //is still using Prospective Table.
        //It deletes from Prospective once the UpdateIntervwItemCandidates process is over
        public async Task<bool> ConvertProspectiveNoDeleteFromProspective(ICollection<int> interviewItemCandidateIds, string Username)
        {
            var prospectiveCandidateIds = await _context.IntervwItemCandidates
                .Where(x => interviewItemCandidateIds.Contains(x.Id))
                .Select(x => x.ProspectiveCandidateId)    
            .ToListAsync();

            if(prospectiveCandidateIds.Count == 0) return false;  

            var prospectives = await _context.ProspectiveCandidates.Where(x => 
                prospectiveCandidateIds.Contains(x.Id)).ToListAsync();
            var interviewItemCandidates = await _context.IntervwItemCandidates.Where(x => 
                prospectiveCandidateIds.Contains(x.ProspectiveCandidateId)).ToListAsync();

            foreach(var prospective in prospectives) {
                //check unique values of PP and Aadhar
                var user = await _userManager.FindByEmailAsync(prospective.Email);
                var charPosition= !prospective.CandidateName.Contains(' ') ? prospective.CandidateName.IndexOf("-", StringComparison.Ordinal):0;

                var prospectiveKnownAs = charPosition <=0 ? prospective.CandidateName : prospective.CandidateName[..charPosition];
                
                if(user == null) {     
                    user = new AppUser
                    {       
                        KnownAs = prospectiveKnownAs,
                        Gender = prospective.Gender,
                        PhoneNumber = prospective.PhoneNo,
                        Email = prospective.Email,
                        UserName = prospective.Email
                    };
                    var result = await _userManager.CreateAsync(user, "newPassword0#");
                    if (!result.Succeeded) return false;
                }

                //create roles
                //var succeeded = await _roleManager.CreateAsync(new AppRole{Name="Candidate"});
                var roleResult = await _userManager.AddToRoleAsync(user, "Candidate");
                //if (!roleResult.Succeeded) return null;
                       
                var userphones = new List<UserPhone>{new() {MobileNo=prospective.PhoneNo, IsMain=true, IsValid=true}};
                if(!string.IsNullOrEmpty(prospective.AlternateNumber)) {
                    var userph=new UserPhone{MobileNo=prospective.AlternateNumber, IsMain=false, IsValid=true};
                    userphones.Add(userph);
                }

                var userprofessions = new List<UserProfession>{new() {ProfessionId=prospective.ProfessionId}};
                var cvDto = new RegisterDto{
                    Gender="M", FirstName = prospective.CandidateName, KnownAs = prospectiveKnownAs, 
                    Username = user.UserName, Email = prospective.Email, AppUserId = user.Id,
                    ReferredByName = prospective.Source, UserProfessions=userprofessions, UserPhones=userphones,
                    City = prospective.CurrentLocation, Nationality = prospective.Nationality,
                    Source = prospective.Source, Address=prospective.Address, Pin=""
                };

                if (!string.IsNullOrEmpty(prospective.Age)) {
                    var age = prospective.Age[..2];
                    cvDto.DOB = DateTime.Today.AddYears(-Convert.ToInt32(age));
                }
            
                // finally, create the object candidate
                var cand = await _candidateRepository.CreateCandidateAsync(cvDto, Username);
                
                //update IntervwItemCandidates.ContactId
                var itemCandidate = interviewItemCandidates.Where(x => x.ProspectiveCandidateId==prospective.Id).FirstOrDefault();
                if(itemCandidate != null) {
                    itemCandidate.CandidateId=cand.Id;
                    itemCandidate.ApplicationNo=cand.ApplicationNo;
                    itemCandidate.PersonId = prospective.PersonId;
                    _context.Entry(itemCandidate).State = EntityState.Modified;
                }

            }
               
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<PagedList<ProspectiveBriefDto>> GetProspectivePagedList([FromQuery]ProspectiveCandidateParams pParams)
        {
            var qry = _context.ProspectiveCandidates.AsQueryable();
            var contactresults = new List<string>();
               
            if(pParams.Id != 0) {
                qry = qry.Where(x => x.Id == pParams.Id);
            } else {
                if(!string.IsNullOrEmpty(pParams.CategoryRef) ) qry = qry.Where(x => x.CategoryRef == pParams.CategoryRef);
                
                if(!string.IsNullOrEmpty(pParams.Search)) qry = qry.Where(x => x.CategoryRef.Contains(pParams.Search));
                if(!string.IsNullOrEmpty(pParams.PhoneNo)) qry = qry.Where(x => x.PhoneNo ==pParams.PhoneNo);
                if(!string.IsNullOrEmpty(pParams.CandidateName)) qry = qry.Where(x => x.CandidateName.Contains(pParams.Search));
                
                switch(pParams.StatusClass.ToLower()) {
                    case "active":
                        qry = qry.Where(x => new List<string>{"wrong Number", "not responding", "will revert later", null}.Contains(x.Status));
                        //contactresults.AddRange(new List<string>{"wrong Number", "not responding", "will revert later", null});
                        break;
                    case "declined":
                        qry = qry.Where(x => x.Status.ToLower().Contains("declined"));
                        //contactresults.AddRange(new List<string>{"declined for overseas", "declined-family issues", "declined-low remuneration", "declined-sc not accepted"});
                        break;
                    case "interested":
                        qry = qry.Where(x => new List<string>{"interested, but negotiate salary", "interested, but doubtful", "interested, and keen"}.Contains(x.Status));
                        //contactresults.AddRange(new List<string>{"interested, but negotiate salary", "interested, but doubtful", "interested, and keen"});
                        break;
                    default:
                        break;
                }
                
                if(pParams.DateRegistered.Year > 2000) qry = qry.Where(x => x.DateRegistered == pParams.DateRegistered);
               
                qry = pParams.Sort.ToLower() switch
                {
                    "date" => qry.OrderBy(x => x.DateRegistered).ThenBy(x => x.Status),
                    //"city" => (IQueryable<ProspectiveCandidate>)qry.OrderBy(x => x.City).ThenBy(x => x.Status),
                    "status" => qry.OrderBy(x => x.Status).ThenBy(X => X.CategoryRef),
                    "name" => (IQueryable<ProspectiveCandidate>)qry.OrderBy(x => x.CandidateName).ThenBy(x => x.Status),
                    "categoryref" => qry.OrderBy(x => x.CategoryRef).ThenBy(x => x.Status),
                    _ => qry.OrderBy(x => x.CategoryRef).ThenBy(x => x.Status),
                };
            }
            
            var totalCount = await qry.CountAsync();

            var paged = await PagedList<ProspectiveBriefDto>.CreateAsync(qry.AsNoTracking()
                    .ProjectTo<ProspectiveBriefDto>(_mapper.ConfigurationProvider),
                    pParams.PageNumber, pParams.PageSize);
                    
            foreach(var pg in paged) {
                if(pg.StatusDate.Year < 1000) pg.StatusDate=pg.DateRegistered;
            }
            return paged;
        }

        public async Task<ICollection<ProspectiveBriefDto>> GetProspectiveList(string orderno, string statusClass)
        {
            var qry = _context.ProspectiveCandidates.AsQueryable();
            
            if(!string.IsNullOrEmpty(orderno) ) qry = qry.Where(x => x.CategoryRef.StartsWith(orderno));
            
            switch(statusClass.ToLower()) {
                case "active":
                    qry = qry.Where(x => new List<string>{"wrong Number", "not responding", "will revert later", null}.Contains(x.Status));
                    //contactresults.AddRange(new List<string>{"wrong Number", "not responding", "will revert later", null});
                    break;
                case "declined":
                    qry = qry.Where(x => x.Status.ToLower().Contains("declined"));
                    //contactresults.AddRange(new List<string>{"declined for overseas", "declined-family issues", "declined-low remuneration", "declined-sc not accepted"});
                    break;
                case "interested":
                    qry = qry.Where(x => new List<string>{"interested, but negotiate salary", "interested, but doubtful", "interested, and keen"}.Contains(x.Status));
                    //contactresults.AddRange(new List<string>{"interested, but negotiate salary", "interested, but doubtful", "interested, and keen"});
                    break;
                default:
                    break;
            }
         
            var totalCount = await qry.CountAsync();

            var lst = await qry.ToListAsync();            

            /*foreach(var pg in lst) {
                if(pg.StatusDate.Year < 1000) pg.StatusDate=pg.DateRegistered;
            } */
            
            var dto = _mapper.Map<ICollection<ProspectiveBriefDto>>(lst);
            return dto;
        }

        public async Task<ICollection<ProspectiveSummaryDto>> GetProspectiveSummary(ProspectiveSummaryParams pParams)
        {
            var qry = _context.ProspectiveCandidates
                    .GroupBy(g => new {g.CategoryRef, g.Source , g.DateRegistered, g.Status})
                    .Where(g => g.Count() > 0)
                    .Select(g => new{g.Key, count = g.Count()})
                    .AsQueryable();

               var objTotal = await qry.ToListAsync();
               
               var summary = new List<ProspectiveSummaryDto>();
               foreach(var q in objTotal) {
                    var summaryItem = summary.Find(x => x.CategoryRef==q.Key.CategoryRef && x.Source == q.Key.Source &&
                        DateOnly.FromDateTime(x.DateRegistered) == DateOnly.FromDateTime(Convert.ToDateTime(q.Key.DateRegistered)));
                    if (summaryItem == null) {
                            var sNew = new ProspectiveSummaryDto
                            {
                                CategoryRef = q.Key.CategoryRef,
                                DateRegistered = (DateTime)q.Key.DateRegistered,
                                Source = q.Key.Source,
                                Status = q.Key.Status
                            };

                            if (q.Key.Status.ToLower()=="notinterested") {
                                    sNew.NotInterested=q.count;
                            } else if (q.Key.Status.ToLower()=="notresponding") {
                                sNew.NotResponding=q.count;
                            } else if(q.Key.Status.ToLower()=="pending") {
                                sNew.Pending=q.count;
                            } else if (q.Key.Status.ToLower()=="concluded") {
                                sNew.Concluded=q.count;
                            } else if (q.Key.Status.ToLower()=="ppissues") {
                                sNew.PpIssues=q.count;
                            } else if (q.Key.Status.ToLower()=="phonenumberwrong") {
                                sNew.PhoneNoWrong=q.count;
                            } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                                sNew.PhoneNotReachable=q.count;
                            } else if (q.Key.Status.ToLower()=="scnotacceptable") {
                                sNew.ScNotAcceptable=q.count;
                            } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                                sNew.PhoneNotReachable=q.count;
                            } else if (q.Key.Status.ToLower()=="salaryofferedislow") {
                                sNew.LowSalary=q.count;
                            } else {
                                sNew.Others=q.count;
                            }

                            summary.Add(sNew);
                        } else 
                        {
                            if (q.Key.Status.ToLower()=="notinterested") {
                                summaryItem.NotInterested+=q.count;
                            } else if (q.Key.Status.ToLower()=="notresponding") {
                                summaryItem.NotResponding+=q.count;
                            } else if(q.Key.Status.ToLower()=="pending") {
                                summaryItem.Pending+=q.count;
                            } else if (q.Key.Status.ToLower()=="concluded") {
                                summaryItem.Concluded+=q.count;
                            } else if (q.Key.Status.ToLower()=="ppissues") {
                                summaryItem.PpIssues+=q.count;
                            } else if (q.Key.Status.ToLower()=="phonenumberwrong") {
                                summaryItem.PhoneNoWrong+=q.count;
                            } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                                summaryItem.PhoneNotReachable+=q.count;
                            } else if (q.Key.Status.ToLower()=="scnotacceptable") {
                                summaryItem.ScNotAcceptable+=q.count;
                            } else if (q.Key.Status.ToLower()=="phonenotreachable") {
                                summaryItem.PhoneNotReachable+=q.count;
                            } else if (q.Key.Status.ToLower()=="salaryofferedislow") {
                                summaryItem.LowSalary+=q.count;
                            } else {
                                summaryItem.Others+=q.count;
                            }
                        }
                }
               
               //var oPaged = summary.Take(sParams.PageSize).Skip(sParams.PageIndex-1).ToList();

               foreach(var smr in summary)
               {
                    smr.Total = smr.AskedToReachHimLater+smr.Concluded+smr.LowSalary+smr.NotInterested+smr.NotResponding+smr.Others+
                         smr.Pending+smr.PhoneNotReachable+smr.PhoneNoWrong+smr.PhoneUnanswered+smr.PpIssues+smr.ScNotAcceptable;
               }
               //var pagination = new Pagination<ProspectiveSummaryDto>(sParams.PageIndex, sParams.PageSize, summary.Count, oPaged) ;
               return summary;
        }

        private async Task<UserProfession> GetUserProfessionFromCategoryRef(string categoryref)
          {
               if(string.IsNullOrEmpty(categoryref)) return null;
               
               //take out the orderno and srno from the string, which are separated by "-"
               int i = categoryref.IndexOf("-");
               if (i== -1) return null;
               var orderno = categoryref[..i];
               var srno = categoryref[(i + 1)..];
               if (string.IsNullOrEmpty(orderno)) return null;
               if (string.IsNullOrEmpty(srno)) return null;
               int iorderno = Convert.ToInt32(orderno);
               int isrno = Convert.ToInt32(srno);

               var qry = await (from o in _context.Orders where o.OrderNo == iorderno 
                    join item in _context.OrderItems on o.Id equals item.OrderId 
                    join c in _context.Professions on item.ProfessionId equals c.Id
                    select new {item.ProfessionId, c.ProfessionName}).FirstOrDefaultAsync();
              
               if (qry == null) return null;

               return new UserProfession{ProfessionId=qry.ProfessionId, ProfessionName=qry.ProfessionName };

          }

        private int GetCustomerIdIdFromCustomerName (string agencyname)
        {
            var id = _context.Customers
                .Where(x => x.CustomerName.ToLower() == agencyname.ToLower())
                .Select(x => x.Id)
                .FirstOrDefault();
            return id;
        }

        public Task<ProspectiveCandidate> GetProspectiveCandidate(ProspectiveSummaryParams pParams)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteProspectiveCandidate(int ProspectiveId)
        {
            var obj = await _context.ProspectiveCandidates.FindAsync(ProspectiveId);

            if (obj == null) return false;

            _context.ProspectiveCandidates.Remove(obj);

            return await _context.SaveChangesAsync() > 0;
        }

        /*public async Task<ICollection<ProspectiveReturnDto>> ConvertProspectiveToCandidates(ICollection<int> prospectiveids, string Username)
        {
            var returnDto = new List<ProspectiveReturnDto>();

            foreach(var id in prospectiveids) {
                var dto = await ConvertProspectiveToCandidate(id, Username);
                returnDto.Add(dto);
            }

            return returnDto;
        }*/
    
        public async Task<ICollection<ProspectiveHeaderDto>> GetProspectiveHeaders(string status)
        {
            var dto = new List<ProspectiveHeaderDto>();

            if(status.ToLower() == "active") {
    
                dto = await (from prospect in  _context.ProspectiveCandidates 
                    where !prospect.Status.ToLower().Contains("declined")
                select new ProspectiveHeaderDto {
                    Orderno = prospect.CategoryRef.Substring(0, prospect.CategoryRef.IndexOf("-") - 1)})       
                .Distinct()
                .ToListAsync();
            } else {
                dto = await (from prospect in  _context.ProspectiveCandidates 
                    where prospect.Status.ToLower().StartsWith(status.ToLower())
                    select new ProspectiveHeaderDto {Orderno = prospect.CategoryRef.Substring(0, prospect.CategoryRef.IndexOf("-")-1)})
                .Distinct()
                .ToListAsync();
            }

            return dto;
        }

        public async Task<ICollection<ComposeCallRecordMessageDto>> ComposeCallRecordMessages(ICollection<ComposeCallRecordMessageDto> dtos, string loggedInUsername)
        {
            var audioMessages = new List<AudioMessage>();
            var audioMessage = new AudioMessage();
            var returnDtos = new List<ComposeCallRecordMessageDto>();
            var returnDto = new ComposeCallRecordMessageDto();

            string CandidateTitle="", CandidateName="", PhoneNo="", EmailId="", Response="";
            string msgBody = "", Opportunity = "", recipientUsername = "";
            DateTime _today = DateTime.UtcNow;
            var messages = new List<Message>();

            var newLine = Environment.NewLine;
            
            foreach(var dto in dtos) {
                CandidateTitle = dto.CandidateTitle;
                CandidateName = dto.CandidateName;
                PhoneNo = dto.PhoneNo;
                EmailId = dto.EmailId;
                Response = dto.CandidateResponse.ToLower();
                Opportunity = dto.Subject;
                var candCreated = await ConvertProspectiveToCandidate(dto.ProspectiveId, dto.KnownAs, dto.EmailId, loggedInUsername);   //also creates AppUser
                    
                recipientUsername = candCreated.CandidateUsername;   // dto.CandidateUsername ?? await _context.GetAppUserIdOfCandidate

                msgBody = CandidateTitle + " " + CandidateName + "<br>Phone No.:" + PhoneNo;    // + "<br>Email Id:" + EmailId;
                msgBody += "<br><br>Dear Mr. " + CandidateName + "<br><br>";
                msgBody += "<ul><b>Subject:" + Opportunity + "</ul></b><br><br>";
                
                switch(dto.ModeOfAdvise.ToLower()) {
                    case "mail":
                    if(Response == "interested, and keen") {
                        msgBody += "Thank you for confirming your interest in the above opportunity. We are accordingly proceeding with " +
                                "processing your profile with the client and will soon revert with client's response";
                    } else if (Response == "interested, but doubtful" ) {
                        msgBody += "Thank you for showing interest in the above opportunity.  We are accordingly proceeding with " 
                            + "processing your application with the client and will soon revert with client's response";
                    } else if(Response == "interested, undecided") {
                        msgBody += "Thank you for the courtesy extended when we spoke to you concerning the above opportunity.  " +
                                        "Kindly confirm your interest within 3 days from today so as to mark you as interested for the job.";
                    } else if(Response == "declined-low remuneration") {
                        msgBody += "Thank you for the courtesy extended when we spoke to you " +
                            "concerning the above opportunity.  We note you have declined the opportunity due to low remuneration." +
                                            "  Should the client agree to enhance their remuneration offer, we will revert to you for reconsideration.";
                    } else if (Response == "declined for overseas") {
                        msgBody += "Thank you for the courtesy extended when we spoke to you " +
                            "concerning the above opportunity.  We regret to note you have declined the opportunity because " +
                            "you are not interested to work overseas.  We have accordingly updated our records and in future you will not "
                            + "be approached for any overseas opportunity.";
                     } else if (Response == "declined - sc not agreed") {
                        msgBody += "Thank you for the courtesy extended when we spoke to you " +
                            "concerning the above opportunity.  We regret to note you have declined the opportunity due to commercial terms.  "
                            + "We have suitably updated our records, and will approach you whenever new opportunity is available suitable to your needs.";
                    } else if (Response == "declined - other reasons") {
                        msgBody += "Thank you for the courtesy extended when we spoke to you " +
                            "concerning the above opportunity.  We regret to note you have declined to accept the above opportunity and " +
                            "have not offered any reason.  We have suitably updated our records.<br><br>Thank you for your time.";
                    } else if(Response == "wrong number") {
                        msgBody += "We tried to reach you for the above opportunity on your given number, but the number is not correct. " +
                            "Please advise your interest in the above opportunity and provide your correct telephone number to update our records.";
                    } else if (Response == "not responding") {
                        msgBody += "In connection with the above opportunity, we tried to reach you on your above mentioned number, " +
                            "but we did not get any response.  Kindly advise your interest in the above opportunity " +
                            "as soon as possible.";
                    }

                    var message = new Message
                    {
                        SenderUsername=loggedInUsername,
                        //RecipientAppUserId=recipientObj.Id,
                        //SenderAppUserId=senderObj.Id,
                        SenderEmail= _config["RAEmailId"] ?? "",
                        RecipientUsername = dto.CandidateUsername ?? "",
                        RecipientEmail = dto.EmailId ?? "",
                        //CCEmail = HRSupobj?.Email ?? "",
                        Subject = dto.Subject,
                        Content = msgBody,
                        MessageType = "CallRecordResponse",
                        MessageComposedOn = _today
                    };

                    messages.Add(message);
                    returnDto.CandidateResponse = dto.CandidateResponse;
                    break;
                    
                    case "phone": case "sms":
                        if(dto.ModeOfAdvise.ToLower() == "phone") {
                            msgBody = msgBody.Replace("<br>", newLine);
                            msgBody = msgBody.Replace("/ul>","");
                            msgBody = msgBody.Replace("<ul>", "");
                            msgBody = msgBody.Replace("</b>", "");
                            msgBody = msgBody.Replace("<b>", "");
                        }
                        
                        switch(Response) {
                            case "interested, and keen":
                                msgBody +="You have confirmed your interest in above opening. We are proceeding with " +
                                    "processing your profile and will soon revert with client's response";
                                break;
                            case "interested, but doubtful":
                                msgBody +="You have confirmed your interest in above opening.  We are proceeding with processing " +
                                    "of your application and will soon revert with client's response";
                                break;
                            case "interested, undecided":
                                msgBody +="In connection with above opportunity, kindly advise your interest within 3 days, so as to retain your profile " +
                                      "Kindly confirm your interest within 3 days from today so as to mark you as interested for the job.";
                                break;
                            case "declined-low remuneration":
                                msgBody +="We note you have declined to be considered for the above job due to low remuneration. " +
                                    "Should the client enhance their remuneration, we will revert to you for reconsideration.";
                                break;
                            case "declined for overseas":
                                msgBody += "We note you have declined to be considered for the above job as you are not interested for overseas openings.  " +
                                    "We have updated our records accordingly and you will not be approached for overseas openings again.";
                                break;
                            case "declined - sc not agreed":
                                msgBody += "We note you have declined to be considered for the above openings due to commercial reasons.  We will approach you " +
                                    "for other openings when the terms are better.";
                                break;
                            case "declined - other reasons":
                                msgBody += "We note you have declined to be considered for the above opening and have not given any reasons for the same. " +
                                        "We have accordingly updated our records.";
                                break;
                            default:
                                break;
                        }
                        returnDto.CandidateResponse = dto.CandidateResponse;
                        if(dto.ModeOfAdvise.ToLower()=="phone") {
                            audioMessage = new AudioMessage {
                                RecipientUsername = recipientUsername,
                                SenderUsername = loggedInUsername,
                                CandidateName = dto.CandidateName,
                                MessageText = msgBody,
                                DateComposed = DateTime.UtcNow,
                                ApplicationNo = candCreated.ApplicationNo,
                                Subject = dto.Subject
                            };
                            audioMessages.Add(audioMessage);
                        } 
                    returnDto.CandidateResponse = dto.CandidateResponse;
                    break;

                    default:
                        break;
                }
                
                foreach(var msg in messages) {
                    _context.Messages.Add(msg);
                }
                foreach(var audio in audioMessages) {
                    _context.AudioMessages.Add(audio);
                }

                returnDto = dto;
                returnDto.MessageText = msgBody;
                returnDtos.Add(returnDto);
                
            }

            var recAffected = await _context.SaveChangesAsync();
            returnDto.MessageText += recAffected + returnDto.MessageText;
            
            return returnDtos;
        }
        public async Task<bool> InsertAudioFiles(ICollection<AudioMessage> audioMessages)
        {
            foreach(var msg in audioMessages) {
                _context.AudioMessages.Add(msg);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<AudioMessageDto>> GetAudioMessagePagedList(AudioMessageParams pParams)
        {
            var qry = _context.AudioMessages.AsQueryable();
               
            if(pParams.ApplicationNo != 0) {
                qry = qry.Where(x => x.ApplicationNo == pParams.ApplicationNo);
            } else if (!string.IsNullOrEmpty(pParams.SenderUsername)) {
                qry = qry.Where(x => x.SenderUsername == pParams.SenderUsername);
            } else {
                if(!string.IsNullOrEmpty(pParams.CandidateName) ) qry = qry.Where(x => x.CandidateName == pParams.CandidateName);
                if(!string.IsNullOrEmpty(pParams.RecipientUsername)) qry = qry.Where(x => x.RecipientUsername == pParams.RecipientUsername);
                if(pParams.FeedbackReceived > 0) qry = qry.Where(x => x.FeedbackReceived == pParams.FeedbackReceived);
                
                if(pParams.DateComposed.Year > 2000) qry = qry.Where(x => x.DateComposed.Year == pParams.DateComposed.Year);
            }
            
            var totalCount = await qry.CountAsync();

            var paged = await PagedList<AudioMessageDto>.CreateAsync(qry.AsNoTracking()
                    .ProjectTo<AudioMessageDto>(_mapper.ConfigurationProvider),
                    pParams.PageNumber, pParams.PageSize);
            
            foreach(var pg in paged) {
                pg.MessageText = pg.MessageText.Replace("<br>", Environment.NewLine);
                pg.MessageText = pg.MessageText.Replace("/ul>","");
                pg.MessageText = pg.MessageText.Replace("<ul>", "");
                pg.MessageText = pg.MessageText.Replace("</b>", "");
                pg.MessageText = pg.MessageText.Replace("<b>", "");
            }
            return paged;
        }

        public async Task<bool> SetAudioText(SetAudioText AudioText) {
            var record = await _context.AudioMessages.Where(x => x.Id == AudioText.Id).FirstOrDefaultAsync();

            if(record == null) return false;
            record.MessageText = AudioText.TextMessage;
            _context.Entry(record).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}