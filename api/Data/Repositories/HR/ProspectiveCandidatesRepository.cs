using System.Runtime.CompilerServices;
using api.DTOs;
using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Identity;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.HR;
using api.Params.Admin;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
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


        public async Task<int> ConvertProspectiveToCandidate(int prospectiveId, string username)
        {
            var prospective = await _context.ProspectiveCandidates.FindAsync(prospectiveId);

            //check unique values of PP and Aadhar
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
                    if (!result.Succeeded) return 0;
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

               if (cand == null) return 0;
               
               //once succeeded, delete the record from prospective list.
               _context.ProspectiveCandidates.Remove(prospective);
               _context.Entry(prospective).State=EntityState.Deleted;

               await _context.SaveChangesAsync();

               return cand.ApplicationNo;
        }

        public async Task<PagedList<ProspectiveBriefDto>> GetProspectivePagedList(ProspectiveCandidateParams pParams)
        {
            var qry = _context.ProspectiveCandidates.AsQueryable();
            var contactresults = new List<string>();
               
            if(pParams.Id != 0) {
                qry = qry.Where(x => x.Id == pParams.Id);
            } else {
                if(!string.IsNullOrEmpty(pParams.CategoryRef) ) qry = qry.Where(x => x.CategoryRef == pParams.CategoryRef);
                
                if(!string.IsNullOrEmpty(pParams.Search)) qry = qry.Where(x => x.CategoryRef == pParams.Search);
                
                if(!string.IsNullOrEmpty(pParams.StatusClass)) {
                    if(pParams.StatusClass=="Active") {
                        contactresults.Add("wrong Number");
                        contactresults.Add("not responding");
                        contactresults.Add("will revert later");
                        contactresults.Add(null);
                    } else if(pParams.StatusClass == "Declined") {
                        contactresults.Add("declined for overseas");
                        contactresults.Add("declined-family issues");
                        contactresults.Add("declined-low remuneration");
                        contactresults.Add("declined-sc not accepted");
                    } else if(pParams.StatusClass=="Interested") {
                        contactresults.Add("interested, but negotiate salary");
                        contactresults.Add("interested, but doubtful");
                        contactresults.Add("interested, and keen");
                    } 

                    if(pParams.Status != "all") qry = qry.Where(x => contactresults.Contains(x.Status.ToLower()));
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

    }
}