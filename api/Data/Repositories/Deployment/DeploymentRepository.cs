using System;
using System.Data.Common;
using System.Linq;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Deployments;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Deployments;
using api.Interfaces.HR;
using api.Interfaces.Messages;
using api.Interfaces.Orders;
using api.Params.Deployments;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private const int _vISA_DOCS_SUBMITTED=600;
        private const int _vISA_REJECTED=800;
        private const int _vISA_ISSUED=700;
        private const int _eMIGRATION_CLEARED=1100;
        private const int _eMIGRATION_DENIED=1200;
        private const int _tICKET_BOOKED=1300;
        private const int _oFFER_ACCEPTED=150;
        private readonly IComposeMessagesHRRepository _composeHR;
        private readonly IComposeMsgsForCandidates _composeCandMsg;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISelDecisionRepository _selRepo;
        public DeploymentRepository(DataContext context, 
            IComposeMessagesHRRepository composeHR, ISelDecisionRepository selRepo, 
            IComposeMsgsForCandidates composeCandMsg, IMapper mapper, 
            UserManager<AppUser> userManager)
        {
            _selRepo = selRepo;
            _userManager = userManager;
            _composeCandMsg = composeCandMsg;
            _composeHR = composeHR;
            _mapper = mapper;
            _context = context;
        }

        private async Task<bool> UpdateDepStatus(int depId, bool save) {
                
                var dep = await _context.Deps.Include(x => x.DepItems).Where(x => x.Id == depId).FirstOrDefaultAsync();
                var lastSeq = dep.DepItems.OrderByDescending(x => x.Sequence).Select(x => x.Sequence).FirstOrDefault();

                if(lastSeq == 5000) {       //concluded
                    dep.CurrentStatus = "Concluded";
                } else {
                    dep.CurrentStatus = await _context.GetNextDepStatusName(lastSeq);
                }
                
                _context.Entry(dep).State = EntityState.Modified;

                if(save) await _context.SaveChangesAsync();

                return true;
        }

        private async Task<DeploymentPendingBriefDto> GetDepPendingDto(int depId) {

            var qry = await (from dp in _context.Deps where dp.Id==depId
                join depitem in _context.DepItems on dp.Id equals depitem.DepId orderby depitem.Sequence descending
                join orderitem in _context.OrderItems on dp.OrderItemId equals orderitem.Id
                join cat in _context.Professions on orderitem.ProfessionId equals cat.Id 
                join order in _context.Orders on orderitem.OrderId equals order.Id
                join cvref in _context.CVRefs on dp.CvRefId equals cvref.Id
                join cand in _context.Candidates on cvref.CandidateId equals cand.Id 
                select new DeploymentPendingBriefDto {
                    DepId = dp.Id, ApplicationNo=cand.ApplicationNo, CandidateName=cand.FullName, 
                    CategoryName=cat.ProfessionName, CityOfWorking=order.CityOfWorking,
                    CustomerId=order.CustomerId, CustomerName=order.Customer.CustomerName, 
                    CvRefId=cvref.Id, DeploySequence=depitem.Sequence, Ecnr=cand.Ecnr=="true",
                    NextSequence=depitem.NextSequence, NextStageDate=depitem.NextSequenceDate,
                    OrderDate=order.OrderDate, OrderItemId=orderitem.Id, OrderNo=order.OrderNo,
                    ReferredOn=cvref.ReferredOn, SelectedOn=dp.SelectedOn
                }).FirstOrDefaultAsync();
            
            return qry;
        }
        public async Task<DepPendingDtoWithErr> AddDeploymentItems(ICollection<DepItem> dto, AppUser user)
        {
            int itemsWithErr=0, itemsSucceeded=0;
            int SeqToCompare=0;
            var DepItemsAdded = new List<DepItem>();
            var candDetail = new CandidateAdviseDto();

            var returnDto = new DepPendingDtoWithErr();

            if(_deployStatuses == null || _deployStatuses.Count==0) _deployStatuses = await GetDeploymentStatusData();

            foreach(var item in dto) {

                //create candidateObject for composing msgs at the endof this loop
                switch(item.Sequence) {
                    
                    case _mEDICALLY_FIT: case _mEDICALLY_UNFIT: case _vISA_REJECTED: case _vISA_ISSUED:
                    case _eMIGRATION_CLEARED: case _eMIGRATION_DENIED: case _tICKET_BOOKED: case _oFFER_ACCEPTED:
                        var candidateAppUsername = await _context.GetAppUsernameFromDepId(_selRepo, item.DepId);   //returns RturnStringDto
                        if(string.IsNullOrEmpty(candidateAppUsername.SuccessString)) {
                            returnDto.ErrorString += ", " + candidateAppUsername?.ErrorString + " does not have Username defined";
                            continue;
                        }
                        var recipientObj = await _userManager.FindByNameAsync(candidateAppUsername.SuccessString);
                        
                        if(recipientObj == null) {
                            returnDto.ErrorString = "Failed to retrieve User Identity of the candidate";
                            //**TODO** create new appuser if not exist
                            return returnDto;       
                        }

                        candDetail = await (from dep in _context.Deps where dep.Id == item.DepId
                            join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                            join sel in _context.SelectionDecisions on cvref.Id equals sel.CvRefId               
                            join rvw in _context.ContractReviewItems on cvref.OrderItemId equals rvw.OrderItemId
                            join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                            select new CandidateAdviseDto {
                                RecipientObj = recipientObj, CVRefId = cvref.Id,
                                ApplicationNo = sel.ApplicationNo, CandidateId = sel.CandidateId,
                                CandidateTitle = sel.Gender == "F" ? "Ms. " : "Mr. ",
                                CandidateName = sel.CandidateName, CandidateGender=sel.Gender ?? "M",
                                CandidateEmail = recipientObj.Email, CandidateUsername=cand.Username,
                                CustomerName = dep.CustomerName,
                                SelectedAs = sel.SelectedAs ?? rvw.ProfessionName, 
                                HrExecEmail= cand.Email,
                                TransactionDate = DateOnly.FromDateTime(item.TransactionDate)
                            }).FirstOrDefaultAsync();
                        //candDetail.SenderObj=await _userManager.FindByNameAsync(candDetail.CandidateUsername);
                        //if(candDetail.SenderObj==null) continue;
                        break;
                    default:
                        break;
                }//end of loop

                var lastDepItem = await _context.DepItems.Where(x => x.DepId==item.DepId).OrderByDescending(x => x.Sequence).FirstOrDefaultAsync();
                var SequenceShdBe = _deployStatuses.Where(x => x.Sequence == lastDepItem.Sequence).Select(x => x.NextSequence).FirstOrDefault();
                
                SeqToCompare = lastDepItem.Sequence;        //The sequence to compare with proposed sequence in the DepItem
                
                do {
                    var status = _deployStatuses
                        .Where(x => x.Sequence == SeqToCompare)
                        .Select(x => new {x.NextSequence, x.isOptional})
                        .FirstOrDefault();
                    
                    if(SequenceShdBe==status.NextSequence) continue;        //SeqShdBe is  verified, so exit the loop
                    
                    if(!status.isOptional) continue;        //no further checks needed, as SeqShdBe is not optional

                    SeqToCompare = status.NextSequence;
                } while (SequenceShdBe==0);
                
                if(SequenceShdBe != item.Sequence) {
                    int nextSq = _deployStatuses.Where(x => x.Sequence == item.Sequence).Select(x => x.Sequence).FirstOrDefault();
                    if(nextSq == 0) {
                        returnDto.ErrorString = "Failed to get Dep Stage for Next Sequence";
                        continue;
                    }
                    var nextSeqStage =_deployStatuses.Where(x => x.Sequence==nextSq).Select(x => new {x.Sequence, x.isOptional}).FirstOrDefault();
                    if(nextSeqStage==null) {
                        returnDto.ErrorString = "Failed to get the next Deployment stage";
                        continue;
                    }

                    if(nextSeqStage.isOptional && nextSeqStage.Sequence==SequenceShdBe+100) {   //negative Seq is always 100 + earlier +ve status
                            SequenceShdBe +=100;
                    } else {
                
                        if(SequenceShdBe != item.Sequence) {
                            returnDto.ErrorString += ", item with sequence " + 
                                _deployStatuses.Where(x => x.Sequence==item.Sequence).Select(x => x.StatusName).FirstOrDefault() 
                                + " - Expected Sequence is " +   
                                _deployStatuses.Where(x => x.Sequence == SequenceShdBe).Select(x => x.StatusName).FirstOrDefault();
                            itemsWithErr +=1;
                        } else if (lastDepItem.TransactionDate >= item.TransactionDate) {
                            returnDto.ErrorString += ", New Transaction item date " + item.TransactionDate 
                                + " cannot be dated earlier than last Transaction Date " + lastDepItem.TransactionDate;
                        }
                    }
                }

                var depStatus = _deployStatuses.Where(x => x.Sequence == SequenceShdBe).FirstOrDefault();   // await _context.GetNextDepStatus(item.Sequence);
                var depitem = new DepItem{
                    DepId = item.DepId, Sequence = item.Sequence, NextSequence = depStatus.NextSequence,
                    TransactionDate = item.TransactionDate, 
                    NextSequenceDate = item.TransactionDate.AddDays(depStatus.WorkingDaysReqdForNextStage)
                };
                
                DepItemsAdded.Add(depitem);
                 
                if(!string.IsNullOrEmpty(returnDto.ErrorString)) return returnDto;

                //post save actions - created now, but saved after saveAsync();
                //still inside the loop - item in Dto
                if(item.Sequence == _vISA_DOCS_SUBMITTED) {     //no messages to candidates, but assign visas
                    var roles = await _userManager.GetRolesAsync(user);
                    if(roles.ToList().Contains("VisaEdit")) {
                        await PostVisaTransactionforVisaDocSubmitted(item.Id, item.TransactionDate);
                    }
                } else {
                    var msgWithErr = await PostDeploymentTransaction(item.DepId, item.Id, candDetail, item.Sequence, item.TransactionDate);
                    if(msgWithErr?.Messages?.Count > 0) {
                        foreach(var msg in msgWithErr.Messages)
                        _context.Messages.Add(msg);
                    }
                    candDetail = null;
                }

            }       //end of oop item in dto
            
            foreach(var d in DepItemsAdded) {
                _context.Entry(d).State = EntityState.Added;
            }

            var DepItemIdsInserted = new List<DepItemInsertedIdDto>();
            
            try{
                itemsSucceeded = await _context.SaveChangesAsync();

                var depIds = DepItemsAdded.Select(x => x.DepId).Distinct().ToList();
                foreach(var depid in depIds) {
                    await UpdateDepStatus(depid, true);       //dep.CurrentStatus Entry.Status is modified, but not saved
                }
                
                foreach(var d in DepItemsAdded) {
                    DepItemIdsInserted.Add(new DepItemInsertedIdDto{
                        DepId=d.DepId, DepItemId=d.Id, NextSequence=d.NextSequence, Sequence = d.Sequence, NextSequenceDate=d.NextSequenceDate });
                };

                returnDto.DeploymentPendingBriefDtos = await _context
                    .GetDepPendingBriefDtoFromDepItemIds(DepItemsAdded.Select(x => x.Id).ToList());

                
            } catch(DbException ex) {
                if(ex.Message.Contains("IX_DepItem")) {
                    returnDto.ErrorString = "Unique Index violation - CVRefId + Sequence";
                } else {
                    returnDto.ErrorString = ex.Message;
                }
                
            } catch(Exception ex) {
                returnDto.ErrorString = ex.Message;
            }

            return returnDto;

        }
        private async Task<ApiReturnDto> PostVisaTransactionforVisaDocSubmitted(int DepItemId, DateTime TransactionDate)
        {
            var returnDto = new ApiReturnDto();

            var visaTrans = await (from depitem in _context.DepItems where depitem.Id == DepItemId
                join dep in _context.Deps on depitem.DepId equals dep.Id
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                join assign in _context.VisaAssignments on cvref.OrderItemId equals assign.OrderItemId
                join visaitem in _context.VisaItems on assign.VisaItemId equals visaitem.Id
                join visa in _context.VisaHeaders on visaitem.VisaHeaderId equals visa.Id
                select  new VisaTransaction {
                    ApplicationNo = cand.ApplicationNo, CandidateName = cand.FullName,
                    DepItemId = DepItemId, Id = dep.Id, VisaCategory = visaitem.VisaCategoryEnglish,
                    VisaNo = visa.VisaNo, VisaAppSubmitted = TransactionDate, VisaItemId = visaitem.Id
                }).FirstOrDefaultAsync();

            if(visaTrans != null) {
                _context.VisaTransactions.Add(visaTrans);
                await _context.SaveChangesAsync();
            }
            returnDto.ReturnInt = visaTrans.Id; 
            returnDto.ErrorMessage = "";

            return returnDto;
        }
        private async Task<MessageWithError> PostDeploymentTransaction(int DepId, int DepItemId, CandidateAdviseDto candDetail, int Sequence, DateTime TransactionDate) 
        {
            var msgWithErr = new MessageWithError();
            var msgs = new List<Message>();
            var msg = new Message();

            switch(Sequence) {
                case _mEDICALLY_FIT:
                    msgWithErr =  await _composeCandMsg.AdviseCandidate_DeploymentStatus(candDetail, TransactionDate, "MedicallyFit");
                    break;
                case _mEDICALLY_UNFIT:
                    msgWithErr = await _composeCandMsg.AdviseCandidate_DeploymentStatus(candDetail, TransactionDate, "MedicallyUnfit");
                    break;
                
                case _vISA_REJECTED:
                    msgWithErr = await _composeCandMsg.AdviseCandidate_DeploymentStatus(candDetail, TransactionDate, "VisaRejected");
                    break;
                
                case _vISA_ISSUED:
                    msgWithErr = await _composeCandMsg.AdviseCandidate_DeploymentStatus(candDetail, TransactionDate, "VisaIssued");
                    break;
                case _eMIGRATION_CLEARED:
                    msgWithErr = await _composeCandMsg.AdviseCandidate_DeploymentStatus(candDetail, TransactionDate, "EmigrationCleared");
                    break;
                case _tICKET_BOOKED:
                    msgWithErr = await _composeCandMsg.AdviseCandidate_DeploymentStatus(candDetail, TransactionDate, "TicketBooked");
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

        public async Task<DepPendingDtoWithErr> EditDeployment(Dep model)
        {
            var dto = new DepPendingDtoWithErr();
            var depitems = new List<DepItem>();
            var depBriefDto = new List<DeploymentPendingBriefDto>();
            
            var existing = await _context.Deps.Include(x => x.DepItems)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existing == null) {
                dto.ErrorString="No deployment record exists to edit";
                return dto;
            }

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
                    depitems.Add(existingItem);
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
                    depitems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }

                _context.Entry(existing).State = EntityState.Modified;
            }

            try 
            {
                await _context.SaveChangesAsync();
                var depPendingDtos = await _context.GetDepPendingBriefDtoFromDepId(model.Id);
                dto.DeploymentPendingBriefDtos = new List<DeploymentPendingBriefDto> {depPendingDtos};
                
            } catch (DbException ex) {
                dto.ErrorString=ex.Message;
            } catch (Exception ex) {
                dto.ErrorString=ex.Message;
            }
            
            await UpdateDepStatus(model.Id, false);
            
            return dto;
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
     
            //var dto = await DoHousekeepingOfDeployments();
            
            var query = (from dep in _context.Deps //where dep.CurrentStatus != "Concluded"
                //join dep in _context.Deps on depitem.DepId equals dep.Id //where dep.CurrentStatus != "Concluded"
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                join vTrans in _context.VisaTransactions on cvref.Id equals vTrans.CvRefId into visaTrans
                    from vTransaction in visaTrans.DefaultIfEmpty()
                select new DeploymentPendingDto {
                    DepId = dep.Id,
                    ApplicationNo = cv.ApplicationNo,
                    TransactionDate = dep.CurrentStatusDate,
                    CandidateName = cv.FirstName + " " + cv.FamilyName,
                    CategoryName = item.Profession.ProfessionName,
                    CustomerName = order.Customer.KnownAs,
                    Ecnr = cv.Ecnr == "true",
                    CustomerId = order.CustomerId,
                    CvRefId = cvref.Id,
                    SelectedOn = cvref.SelectionStatusDate,
                    ReferredOn = cvref.ReferredOn, 
                    OrderNo = order.OrderNo,
                    CityOfWorking = order.CityOfWorking,
                    OrderDate = order.OrderDate,
                    CurrentStatus =dep.CurrentStatus,
                    OrderItemId = item.Id,
                    VisaTransactionId = vTransaction.VisaItemId,
                    VisaNo = vTransaction.VisaNo
                })
                .OrderByDescending(x => x.ApplicationNo)
                .AsQueryable();
            
            depParams.Status ??= "Concluded";

            if(!string.IsNullOrEmpty(depParams.Status)) 
                query = query.Where(x => x.CurrentStatus.ToLower() != depParams.Status.ToLower());
            if(depParams.OrderNo != 0) {
                query = query.Where(x => x.OrderNo == depParams.OrderNo);
            } else if(depParams.CvRefId != 0) {
                query = query.Where(x => x.CvRefId == depParams.CvRefId);
            } else if(depParams.OrderItemId !=0) {
                query = query.Where(x => x.OrderItemId== depParams.OrderItemId);
            } else if (!string.IsNullOrEmpty(depParams.CandidateName)) {
                query = query.Where(x => x.CandidateName.ToLower().Contains(depParams.CandidateName.ToLower()));
            } else if (depParams.ApplicationNo != 0) {
                query = query.Where(x => x.ApplicationNo == depParams.ApplicationNo);
            } else if (!string.IsNullOrEmpty(depParams.CustomerName)) {
                query = query.Where(x => x.CustomerName.ToLower().Contains(depParams.CustomerName.ToLower()));
            } else if(depParams.SelectedOn.Year > 2000) {
                query = query.Where(x => DateOnly.FromDateTime(x.SelectedOn) == DateOnly.FromDateTime(depParams.SelectedOn));
            }

            if(!string.IsNullOrEmpty(depParams.CurrentStatus)) 
                query = query.Where(x => x.CurrentStatus.ToLower().Contains(depParams.CurrentStatus.ToLower()));

            var paged = await PagedList<DeploymentPendingDto>.CreateAsync(query.AsNoTracking()
                .ProjectTo<DeploymentPendingDto>(_mapper.ConfigurationProvider)
                , depParams.PageNumber, depParams.PageSize);

            //var depids = paged.Select(x => x.DepId).ToList();
            //var depitems = await _context.DepItems.Where(x => depids.Contains(x.DepId)).ToListAsync();

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

        //Dep.CurrentStatus is expected to reflect the latest status of DepItems.
        public async Task<ReturnStringsDto> DoHousekeepingOfDeployments()
        {

            var deps = await _context.Deps.Include(x => x.DepItems)
                .Select(x => new {
                    DepId = x.Id, CurrentStatus = x.CurrentStatus, CurrentStatusDate = x.CurrentStatusDate,
                    depItemLatestStatus = x.DepItems
                        .OrderByDescending(x => x.Sequence).Take(1)
                        .Select(m => new {ItemLatestSeq = m.Sequence, 
                        ItemLatestDate = m.TransactionDate})
                    .FirstOrDefault()})
                .AsNoTracking()
                .ToListAsync();

            var depStatuses = await _context.DeployStatuses.Select(x => new {Seq=x.Sequence, SeqName=x.StatusName}).ToListAsync();

            foreach(var dep in deps) {
                var deployment = await _context.Deps.FindAsync(dep.DepId);
                var stNameShdBe = depStatuses.Find(x => x.Seq == dep.depItemLatestStatus.ItemLatestSeq).SeqName;
                if(dep.CurrentStatus != stNameShdBe) {
                    deployment.CurrentStatus = stNameShdBe;
                    _context.Entry(deployment).State = EntityState.Modified;
                }
                
                if(deployment.CurrentStatusDate != dep.depItemLatestStatus.ItemLatestDate) {
                    deployment.CurrentStatusDate = dep.depItemLatestStatus.ItemLatestDate;
                    _context.Entry(deployment).State = EntityState.Modified;
                }   
            }

            int ct=0;
            if(_context.ChangeTracker.HasChanges()) {
                ct = await _context.SaveChangesAsync();
            }
            var dto = new ReturnStringsDto {
                SuccessString = "A total of " + ct + " Dep records were updated" };

            return dto;
        }
   
    }
}