using System.Data.Common;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Deployments;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Deployments;
using api.Interfaces.HR;
using api.Interfaces.Messages;
using api.Interfaces.Orders;
using api.Params.Deployments;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Deployment
{
    public class DeploymentRepository : IDeploymentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private ICollection<DeployStatus> _deployStatuses;
        private const int _mEDICALLY_FIT=400;
        private const int _mEDICALLY_UNFIT=500;
        private const int _vISA_REJECTED=800;
        private const int _vISA_ISSUED=700;
        private const int _eMIGRATION_CLEARED=1100;
        private const int _eMIGRATION_DENIED=1200;
        private const int _tICKET_BOOKED=1300;
        private const int _oFFER_ACCEPTED=150;
        private readonly IComposeMessagesHRRepository _composeHR;
        private readonly IComposeMsgsForCandidates _composeCandMsg;
        private readonly UserManager<AppUser> _userManager;
        public DeploymentRepository(DataContext context, IComposeMessagesHRRepository composeHR, UserManager<AppUser> userManager,
            IComposeMsgsForCandidates composeCandMsg, IMapper mapper)
        {
            _userManager = userManager;
            _composeCandMsg = composeCandMsg;
            _composeHR = composeHR;
            _mapper = mapper;
            _context = context;
        }

        private async Task<bool> UpdateDepStatus(int depId, bool save) {
                
                var dep = await _context.Deps.Include(x => x.DepItems).Where(x => x.Id == depId).FirstOrDefaultAsync();
                var lastSeq = dep.DepItems.OrderByDescending(x => x.Sequence).Select(x => x.NextSequence).FirstOrDefault();

                if(lastSeq == 5000) {       //concluded
                    dep.CurrentStatus = "Concluded";
                } else {
                    dep.CurrentStatus = await _context.GetNextDepStatusName(lastSeq);
                }
                
                _context.Entry(dep).State = EntityState.Modified;

                if(save) await _context.SaveChangesAsync();

                return true;
        }
        public async Task<DepPendingDtoWithErr> AddDeploymentItems(ICollection<DepItemToAddDto> dto, 
            string Username)
        {
            string strErr="";
            int itemsWithErr=0, itemsSucceeded=0;
            int SeqToCompare=0;
            var items = new List<DepItem>();
            var candDetail = new CandidateAdviseDto();
            var msgWithErr = new MessageWithError();
            var OdIdCandId = new OrderDetailIdCVRefIdCandidateIdDto(); 

            var returnDto = new DepPendingDtoWithErr();

            if(_deployStatuses == null || _deployStatuses.Count==0) _deployStatuses = await GetDeploymentStatusData();

            foreach(var item in dto) {

                msgWithErr = new MessageWithError();

                OdIdCandId = await _context.GetOrderDetailIdCVRefIdCandIdFromDepId(item.DepId);

                //create candidateObject for composing msgs at the endof this loop
                
                switch(item.Sequence) {
                    case _mEDICALLY_FIT:
                    case _mEDICALLY_UNFIT:
                    case _vISA_REJECTED:
                    case _vISA_ISSUED:
                    case _eMIGRATION_CLEARED:
                    case _eMIGRATION_DENIED:
                    case _tICKET_BOOKED:
                    case _oFFER_ACCEPTED:
                    var candidateAppUserId = await _context.GetAppUserIdOfCandidate(OdIdCandId.CandidateId);
                    var recipientObj = await _userManager.FindByIdAsync(candidateAppUserId.ToString());
                    
                    if(recipientObj == null) {
                        msgWithErr.ErrorString = "Failed to retrieve User Identity of the candidate";
                        return null;        //*TODO* - incorporate error strng in return object
                    }

                    var HRExecUsername = await _context.GetHRExecUsernameFromOrderItemId(OdIdCandId.OrderItemId);
                    var CustomerName = await _context.GetCustomerNameFromOrderItemId(OdIdCandId.OrderItemId);
                    var senderObj = await _userManager.FindByNameAsync(HRExecUsername);
                    if(senderObj==null) continue;

                    candDetail = await (from cvref in _context.CVRefs where cvref.Id == OdIdCandId.CvRefId
                        join sel in _context.SelectionDecisions on cvref.Id equals sel.CvRefId               
                        join rvw in _context.ContractReviewItems on cvref.OrderItemId equals rvw.OrderItemId
                        join dep in _context.Deps on cvref.Id equals dep.CvRefId
                        //join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                        select new CandidateAdviseDto {
                            RecipientObj = recipientObj, SenderObj = senderObj, CVRefId = OdIdCandId.CvRefId,
                            ApplicationNo = sel.ApplicationNo, CandidateId = sel.CandidateId,
                            CandidateTitle = sel.Gender == "F" ? "Ms. " : "Mr. ",
                            CandidateName = sel.CandidateName, CandidateGender=sel.Gender ?? "M",
                            CandidateEmail = recipientObj.Email, 
                            CustomerName = CustomerName,
                            SelectedAs = sel.SelectedAs ?? rvw.ProfessionName, HrExecEmail= senderObj.Email,
                            TransactionDate = DateOnly.FromDateTime(item.TransactionDate)
                        }).FirstOrDefaultAsync();
        
                        break;
                    default:
                        break;
                }//end of loop

                var lastDepItem = await _context.DepItems.Where(x => x.DepId==item.DepId)
                    .OrderByDescending(x => x.Sequence).FirstOrDefaultAsync();
                var SequenceShdBe = _deployStatuses.Where(x => x.Sequence == lastDepItem.Sequence).Select(x => x.NextSequence).FirstOrDefault();
                
                SeqToCompare = lastDepItem.Sequence;        //The sequence to compare with proposed sequence in the DepItem

                do {
                    var status = _deployStatuses
                        .Where(x => x.Sequence == SeqToCompare)
                        .Select(x => new {x.NextSequence, x.isOptional})
                        .FirstOrDefault();
                    
                    if(SequenceShdBe==status.NextSequence) continue;        //SeqShdBe is  verified
                    
                    if(!status.isOptional) continue;        //no further checks needed, as SeqShdBe is not optional

                    SeqToCompare = status.NextSequence;
                } while (SequenceShdBe==0);
                
                if(SequenceShdBe != item.Sequence) {
                    strErr += ", item with sequence " + item.Sequence + " - Sequence Expected is: " + SequenceShdBe;
                    itemsWithErr +=1;
                } else if (lastDepItem.TransactionDate >= item.TransactionDate) {
                    strErr += ", New Transaction item date " + item.TransactionDate 
                        + " cannot be dated earlier than last Transaction Date " + lastDepItem.TransactionDate;
                } else {
                    var depStatus = _deployStatuses.Where(x => x.Sequence == SequenceShdBe).FirstOrDefault();   // await _context.GetNextDepStatus(item.Sequence);
                    var depitem = new DepItem{
                        DepId = item.DepId, Sequence = item.Sequence, NextSequence = depStatus.NextSequence,
                        TransactionDate = item.TransactionDate, 
                        NextSequenceDate = item.TransactionDate.AddDays(depStatus.WorkingDaysReqdForNextStage)
                    };
                    items.Add(depitem);
                }

                if(!string.IsNullOrEmpty(strErr)) {
                    returnDto.ErrorString=strErr;
                    return returnDto;
                }

                //post save actions - created now, but saved after saveAsync();
                //still inside the loop - item in Dto
                msgWithErr = await PostDeploymentTransaction(candDetail, item.Sequence, item.TransactionDate);
                if(msgWithErr?.Messages?.Count > 0) {
                    foreach(var msg in msgWithErr.Messages)
                    _context.Messages.Add(msg);
                }
                candDetail = null;

            }       //end of oop item in dto
            
            foreach(var d in items) {
                _context.Entry(d).State = EntityState.Added;
            }

            var DepItemIdsInserted = new List<DepItemAndDepIdDto>();
            
            try{
                itemsSucceeded = await _context.SaveChangesAsync();

                var ids =  items
                    .Select(x => new DepItem { Id = x.Id, DepId = x.DepId})
                    .ToList();

                foreach(var id in ids) {
                    DepItemIdsInserted.Add(new DepItemAndDepIdDto{DepId=id.DepId, DepItemId=id.Id});
                }
                
                var depIds = items.Select(x => x.DepId).Distinct().ToList();
                foreach(var depid in depIds) {
                    await UpdateDepStatus(depid, true);       //dep.CurrentStatus Entry.Status is modified, but not saved
                }
            } catch(DbException ex) {
                if(ex.Message.Contains("IX_DepItem")) {
                    strErr = "Unique Index violation - CVRefId + Sequence";
                } else {
                    strErr = ex.Message;
                }
                
            } catch(Exception ex) {
                strErr = ex.Message;
            }

            var query = await GetDeploymentPendings();

            var dtoToReturn = new List<DeploymentPendingDto>();

            foreach(var item in dto ) {
                var foundItem = query.Where(x => x.DepId == item.DepId).FirstOrDefault();
                if(foundItem != null) dtoToReturn.Add(foundItem);
            }

            returnDto.deploymentPendingDtos=dtoToReturn;
            returnDto.DepItemIdsInserted=DepItemIdsInserted;
            return returnDto;

        }

        private async Task<ICollection<DeploymentPendingDto>> GetDeploymentPendings()
        {
            
            var query = await (from dep in _context.Deps where dep.CurrentStatus != "Concluded"
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                
                select new DeploymentPendingDto {
                    DepId = dep.Id,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    CategoryName = item.Profession.ProfessionName,
                    CustomerName = order.Customer.KnownAs,
                    CvRefId = cvref.Id,
                    SelectedOn = cvref.SelectionStatusDate,
                    ReferredOn = cvref.ReferredOn,
                    OrderNo = order.OrderNo,
                    OrderDate = order.OrderDate,
                    DeploySequence =dep.DepItems.OrderByDescending(x => x.Sequence)
                        .Select(x => x.Sequence).FirstOrDefault(), 
                    NextSequence =dep.DepItems.OrderByDescending(x => x.NextSequence)
                        .Select(x => x.NextSequence).FirstOrDefault(), 
                    OrderItemId = item.Id,
                    NextStageDate = dep.DepItems.OrderByDescending(x => x.TransactionDate)
                        .Select(x => x.NextSequenceDate).FirstOrDefault()
                }).ToListAsync();
            
            return query;
        }

        private async Task<MessageWithError> PostDeploymentTransaction(CandidateAdviseDto candDetail, int Sequence, DateTime TransactionDate) 
        {
            
            var msgWithErr = new MessageWithError();
            var msgs = new List<Message>();
            var msg = new Message();

            switch(Sequence) {
                case _mEDICALLY_FIT:
                    msgWithErr =  _composeCandMsg.AdviseCandidate_MedicallyFit(candDetail, TransactionDate);
                    break;
                case _mEDICALLY_UNFIT:
                    msgWithErr = _composeCandMsg.AdviseCandidate_MedicallyUnfit(candDetail, TransactionDate);
                    break;
                case _vISA_REJECTED:
                    msgWithErr = _composeCandMsg.AdviseCandidate_VisaRejected(candDetail, TransactionDate);
                    break;
                case _vISA_ISSUED:
                    msgWithErr = _composeCandMsg.AdviseCandidate_VisaIssued(candDetail, TransactionDate);
                    break;
                case _eMIGRATION_CLEARED:
                    msgWithErr = _composeCandMsg.AdviseCandidate_EmigrationCleared(candDetail, TransactionDate);
                    break;
                case _tICKET_BOOKED:
                    msgWithErr = await _composeCandMsg.AdviseCandidate_TicketBooked(candDetail, TransactionDate);
                    break;
                case _oFFER_ACCEPTED:
                    msgWithErr = _composeCandMsg.AdviseCandidate_OfferAccepted(candDetail, TransactionDate);
                    break;
                default:

                    break;  
            }
   
            return msgWithErr;
            
        }
        public async Task<string> DeleteDep(int depId)
        {
            string strErr="";
            
            var obj = await _context.Deps.FindAsync(depId);
            if (obj == null) return "The deployment object does not exist";
            
            _context.Deps.Remove(obj); 
            _context.Entry(obj).State = EntityState.Deleted;
            
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

        public async Task<string> DeleteDepItem(int depItemId)
        {
            string strErr="";
            int ct=0;

            var obj = await _context.DepItems.FindAsync(depItemId);
            if (obj == null) return "The Deployment Item does not exist";

            var items = await _context.DepItems.Where(x => x.Sequence == obj.Sequence || x.Sequence > obj.Sequence).ToListAsync();
            
            //verify if CandidateFlight has any records relating to the DepItemId
            var candflightToDelete = await _context.CandidateFlights
                    .Where(x => x.DepItemId == depItemId).FirstOrDefaultAsync();
            if(candflightToDelete != null) {
                _context.CandidateFlights.Remove(candflightToDelete);
                _context.Entry(candflightToDelete).State= EntityState.Deleted;
            }
            
            foreach(var item in items) {
                _context.DepItems.Remove(item);
                _context.Entry(item).State = EntityState.Deleted;
                await UpdateDepStatus(item.DepId, false);
            }
   
             try 
            {
                ct =await _context.SaveChangesAsync();
                var depids = items.Select(x => x.DepId).Distinct().ToList();
                foreach(var depid in depids) {
                    await UpdateDepStatus(depid, true);
                }
                
            } catch (DbException ex) {
                strErr = "Database Error: " + ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            return strErr;
        }
        
        public async Task<bool> DeleteDeploymentAttachment(string fullPath) {
            var depitem = await _context.DepItems.Where(x => x.FullPath.ToLower()==fullPath.ToLower()).FirstOrDefaultAsync();
            if(depitem == null) return false;

            depitem.FullPath = "";
            _context.Entry(depitem).State = EntityState.Modified;
            
            System.IO.File.Delete(fullPath);
            return await _context.SaveChangesAsync() > 0;

            
        }
        public async Task<string> EditDepItem(DepItem depItem)
        {
            string strErr="";
            
            var existingItem = await _context.DepItems.FindAsync(depItem.Id);
            if(existingItem == null) return "The deployment Item to exit is not on record.";

            _context.Entry(existingItem).CurrentValues.SetValues(depItem);

            await UpdateDepStatus(existingItem.DepId, false);

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

        public async Task<string> EditDeployment(Dep model)
        {
            string strErr="";
            
            var existing = await _context.Deps.Include(x => x.DepItems)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existing == null) return "Deployment Object does not exist";

            _context.Entry(existing).CurrentValues.SetValues(model);

            foreach (var existingItem in existing.DepItems.ToList())
            {
                if(!model.DepItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    //if sequence is 1300, delete candidteflight record also
                    if(existingItem.Sequence==1300) {
                        var flight = await _context.CandidateFlights.Where(x => x.CvRefId == existing.CvRefId).FirstOrDefaultAsync();
                        if (flight != null) {
                            _context.CandidateFlights.Remove(flight);
                            _context.Entry(flight).State = EntityState.Deleted;
                        }
                    }
                    _context.DepItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 

                    
                }
            }

             foreach(var newItem in model.DepItems)
            {
                var existingItem = existing.DepItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingItem != null)    //update existing item
                {
                    if(existingItem.TransactionDate != newItem.TransactionDate) newItem.TransactionDate.AddHours(9);
                    if(existingItem.NextSequenceDate != newItem.NextSequenceDate) newItem.NextSequenceDate.AddHours(9);
                        //client changes the transaction date by deducting 8:30 hours, inexplicably.

                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new record
                    var itemToInsert = new DepItem
                    {
                        DepId = existing.Id,
                        TransactionDate = newItem.TransactionDate,
                        NextSequence = newItem.NextSequence,
                        Sequence = newItem.Sequence,
                        NextSequenceDate = newItem.NextSequenceDate
                    };

                    existing.DepItems.Add(itemToInsert);

                    _context.Entry(itemToInsert).State = EntityState.Added;
                }

                _context.Entry(existing).State = EntityState.Modified;
            }
            
            await UpdateDepStatus(model.Id, false);

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

        public async Task<Dep> GetDeploymentByCVRefId(int cvrefid)
        {
            return await _context.Deps.Include(x => x.DepItems).Where(x => x.CvRefId == cvrefid).FirstOrDefaultAsync();
        }

        private async Task<List<int>> GetNextSequenceAndDays(int sequence, bool ecnr)
        {
            int seq = 0;
            int days = 0;

            switch(sequence) {
                case 700:
                    seq = ecnr ? 1100 : 900;
                    days = 2;
                    break;
                default:
                    var seqAnddays = await _context.DeployStatuses.Where(x => x.Sequence == sequence)
                        .Select(x => new {NextSeq=x.NextSequence, days=x.WorkingDaysReqdForNextStage})
                        .FirstOrDefaultAsync();
                    seq = seqAnddays != null ? seqAnddays.NextSeq : 0;
                    days = seqAnddays != null ? seqAnddays.days : 0;
                    break;
            }

            return new List<int> {seq, days};
        }

        public async Task<DepItem> GetNextDepItemToAddFomCVRefId(int cvrefid)
        {
            var deployment = await _context.Deps.Include(x => x.DepItems).Where(x => x.CvRefId == cvrefid).FirstOrDefaultAsync();

            if(deployment == null) return null;

            var lastItem = deployment.DepItems.OrderByDescending(x => x.TransactionDate).Take(1).FirstOrDefault();
            
            var seqAndDays = await GetNextSequenceAndDays(lastItem.Sequence, deployment.Ecnr);

            var item = new DepItem{
                DepId = deployment.Id,
                TransactionDate = DateTime.Now,
                Sequence = lastItem.NextSequence,
                NextSequence = seqAndDays[0],
                NextSequenceDate = DateTime.Now.AddDays(seqAndDays[1])
            };

            return item;
        }

        public async Task<PagedList<DeploymentPendingDto>> GetDeployments(DeployParams depParams)
        {
            
            var query = (from dep in _context.Deps where dep.CurrentStatus != "Concluded"
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                
                select new DeploymentPendingDto {
                    DepId = dep.Id,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    CategoryName = item.Profession.ProfessionName,
                    CustomerName = order.Customer.KnownAs,
                    CvRefId = cvref.Id,
                    SelectedOn = cvref.SelectionStatusDate,
                    ReferredOn = cvref.ReferredOn,
                    OrderNo = order.OrderNo,
                    CityOfWorking = order.CityOfWorking,
                    OrderDate = order.OrderDate,
                    DeploySequence =dep.DepItems.OrderByDescending(x => x.Sequence)
                        .Select(x => x.Sequence).FirstOrDefault(), 
                    NextSequence =dep.DepItems.OrderByDescending(x => x.NextSequence)
                        .Select(x => x.NextSequence).FirstOrDefault(), 
                    OrderItemId = item.Id,
                    NextStageDate = dep.DepItems.OrderByDescending(x => x.TransactionDate)
                        .Select(x => x.NextSequenceDate).FirstOrDefault()
                }).AsQueryable();

            if(depParams.CvRefId != 0) query = query.Where(x => x.CvRefId == depParams.CvRefId);

            //if(depParams.SelectedOn.Year > 2000) query = query.Where(x => DateOnly.FromDateTime(x.SelectedOn) == DateOnly.FromDateTime(depParams.SelectedOn));
            
            if(depParams.OrderItemId != 0 ) 
                query = query.Where(x => x.OrderItemId== depParams.OrderItemId);
            
            //if(depParams.CustomerId > 0) query = query.Where(x => x.CustomerId == depParams.CustomerId);

            var paged = await PagedList<DeploymentPendingDto>.CreateAsync(query.AsNoTracking()
                //.ProjectTo<DeploymentPendingDto>(_mapper.ConfigurationProvider)
                , depParams.PageNumber, depParams.PageSize);
        
            return paged;
        }

        public async Task<ICollection<DeployStatus>> GetDeploymentStatusData()
        {
            return await _context.DeployStatuses.OrderBy(x => x.Sequence).ToListAsync();
        }

        public async Task<ICollection<DeployStatusAndName>> GetDeploymentSeqAndStatus()
        {
            return await _context.DeployStatuses
                .Select(x => new DeployStatusAndName {
                    Id = x.Sequence,
                    Name=x.StatusName
                })
            .OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Dep> GetDeploymentByDepId(int depid)
        {
            var obj = await _context.Deps.Include(x => x.DepItems.OrderByDescending(m => m.TransactionDate))
                .Where(x => x.Id == depid)
                .FirstOrDefaultAsync();

            return obj;
        }

        public async Task<ICollection<FlightDetail>> GetFlightData()
        {
            var obj = await _context.FlightDetails
                .OrderBy(x => x.AirportOfBoarding)
                .ThenBy(x => x.AirportOfDestination)
                .ThenBy(x => x.ETD_Boarding)
                .ToListAsync();
            
            return obj;
        }

        public async Task<DepPendingDtoWithErr> InsertDepItemsWithFlights(ICollection<DepItemWithFlightDto> depItemsWithFlight, string username)
        {
            var dtoToReturn = new DepPendingDtoWithErr();

            var depItemToAddDto = new List<DepItemToAddDto>();
            var candFlights = new List<CandidateFlight>();

            foreach(var depitem in depItemsWithFlight) {
                var item = depitem.DepItem;
                depItemToAddDto.Add(new DepItemToAddDto{DepId = item.DepId, Sequence = item.Sequence, TransactionDate=item.TransactionDate });
            }

            var dto = await AddDeploymentItems(depItemToAddDto, username);
            if(dto == null) {
                dtoToReturn.ErrorString = "Failed to insert deployment items";
                return dtoToReturn;
            }

            var depidAndItemIds = dto.DepItemIdsInserted;

            //after DepItems are writen toDB, retrieve candidateflight details
            if(!string.IsNullOrEmpty(dto.ErrorString)) return dto;
            
            var flight = depItemsWithFlight.Select(x => x.candidateFlight).FirstOrDefault();
            //var recordCount = await GenerateCandidateFlightHeadersFromDepItemId(depItemsWithFlight, dto.DepItemIdsInserted, flight);
            
            var depids = depidAndItemIds.Select(x => x.DepId).ToList();

            var cands = await (from dep in _context.Deps where depids.Contains(dep.Id)
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                select new CandidateFlight {
                    DepId = dep.Id, CvRefId=dep.CvRefId, ApplicationNo=cand.ApplicationNo,
                    CandidateName = cand.FullName, CustomerCity=dep.CityOfWorking, 
                    CustomerName = dep.CustomerName
                }).ToListAsync();

            if(cands.Count == 0) return null;

            foreach(var flt in depItemsWithFlight) {
                var candidate = cands.Where(x => x.DepId == flt.DepItem.DepId).FirstOrDefault();
                if(candidate != null) {
                    flt.candidateFlight.ApplicationNo = candidate.ApplicationNo;
                    flt.candidateFlight.CandidateName=candidate.CandidateName;
                    flt.candidateFlight.CustomerCity = candidate.CustomerCity;
                    flt.candidateFlight.CustomerName = candidate.CustomerName;
                    flt.candidateFlight.CvRefId = candidate.CvRefId;
                    flt.candidateFlight.DepItemId = depidAndItemIds
                        .Where(x => x.DepId == flt.DepItem.DepId).Select(x => x.DepItemId).FirstOrDefault();
                }
                
                _context.CandidateFlights.Add(flt.candidateFlight);
            }

            var ct = await _context.SaveChangesAsync();
            return dto;
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

        public async Task<CandidateFlight> GetCandidateFlightFromCVRefId(int cvRefId)
        {
            var obj = await _context.CandidateFlights.Where(x => x.CvRefId == cvRefId).FirstOrDefaultAsync();

            return obj;
        }

    }
}