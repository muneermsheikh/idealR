using api.DTOs;
using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Identity;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.HR;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.HR
{
    public class ProspectiveCandidatesRepository : IProspectiveCandidatesRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public ProspectiveCandidatesRepository(DataContext context, ITokenService tokenService, 
            IMapper mapper,UserManager<AppUser> userManager, ICandidateRepository candidateRepository)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _candidateRepository = candidateRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ProspectiveReturnDto> ConvertProspectiveToCandidate(int prospectiveId, string username)
        {
            var prospective = await _context.ProspectiveCandidates.FindAsync(prospectiveId);

            if(string.IsNullOrEmpty(prospective.Email)) return null;
            
               var user = await _userManager.FindByEmailAsync(prospective.Email);
               var charPosition= !prospective.CandidateName.Contains(' ') ? prospective.CandidateName.IndexOf("-", StringComparison.Ordinal):0;

               var prospectiveKnownAs = charPosition <=0 ? prospective.CandidateName : prospective.CandidateName[..charPosition];
               
               if(user == null) {     
                    user = new AppUser
                    {
                         //UserType = "Candidate",
                         KnownAs = prospectiveKnownAs,
                         Gender = prospective.Gender,
                         PhoneNumber = prospective.PhoneNo,
                         Email = prospective.Email,
                         UserName = prospective.Email
                    };
                    var result = await _userManager.CreateAsync(user, "newPassword0#");
                    if (!result.Succeeded) return null;
               }

               //create roles
               //var succeeded = await _roleManager.CreateAsync(new AppRole{Name="Candidate"});
               var roleResult = await _userManager.AddToRoleAsync(user, "Candidate");
               //if (!roleResult.Succeeded) return null;
                    
               //var userAdded = await _userManager.FindByEmailAsync(registerDto.Email);
               //no need to retreive obj from DB - the object 'user' can be used for the same
               
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
                    ProspectiveCandidateId = prospectiveId };

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
                          
            /* qry = pParams.Sort.ToLower() switch
            {
                "date" => qry.OrderBy(x => x.DateRegistered).ThenBy(x => x.Status),
                //"city" => (IQueryable<ProspectiveCandidate>)qry.OrderBy(x => x.City).ThenBy(x => x.Status),
                "status" => qry.OrderBy(x => x.Status).ThenBy(X => X.CategoryRef),
                "name" => (IQueryable<ProspectiveCandidate>)qry.OrderBy(x => x.CandidateName).ThenBy(x => x.Status),
                "categoryref" => qry.OrderBy(x => x.CategoryRef).ThenBy(x => x.Status),
                _ => qry.OrderBy(x => x.CategoryRef).ThenBy(x => x.Status),
            }; */
            
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

        public async Task<ICollection<ProspectiveReturnDto>> ConvertProspectiveToCandidates(ICollection<int> prospectiveids, string Username)
        {
            var returnDto = new List<ProspectiveReturnDto>();

            foreach(var id in prospectiveids) {
                var dto = await ConvertProspectiveToCandidate(id, Username);
                returnDto.Add(dto);
            }

            return returnDto;
        }
    
        public async Task<ICollection<ProspectiveHeaderDto>> GetProspectiveHeaders(string status)
        {
            var dto = new List<ProspectiveHeaderDto>();

            if(status.ToLower() == "active") {
    
                dto = await (from prospect in  _context.ProspectiveCandidates 
                    where !prospect.Status.ToLower().Contains("declined")
                select new ProspectiveHeaderDto {
                    Orderno = prospect.CategoryRef.Substring(0,5)})
                .Distinct()
                .ToListAsync();
            } else {
                dto = await (from prospect in  _context.ProspectiveCandidates 
                    where prospect.Status.ToLower().StartsWith(status.ToLower())
                    select new ProspectiveHeaderDto {Orderno = prospect.CategoryRef.Substring(0,5)})
                .Distinct()
                .ToListAsync();
            }

            return dto;
        }
    }
}