using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.Admin.Order;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Interfaces.HR;
using api.Interfaces.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace api.Data.Repositories.HR
{
    public class ComposeMsgsForCandidates : IComposeMsgsForCandidates
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly DateTime _today = DateTime.Now;
        public ComposeMsgsForCandidates(DataContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }


        public MessageWithError AdviseCandidate_EmigrationCleared(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
            var msgWithErr = new MessageWithError();

            var subject = "Your Emigration Clearance is received";
            var subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                " for " + candDetail.CustomerName + " - your Emigration Clearance is received</u>";
      
            var msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                msgBody += "Pleased to advise the Emigration Authorities have cleared your emigration.  " +
                    "As you are aware, your Passport requires Emigration Clearance, " + 
                    "and this procedure was required before you could travel abroad for employment.<br><br>" +
                    "We are now booking your air ticket for your overseas travel, and will advise you once it is booked.<br><br>";

                msgBody += "Best Regards<br>HR Supervisor";

                var message = new Message
                {
                    SenderUsername=candDetail.SenderObj.UserName,
                    //RecipientAppUserId=candDetail.RecipientObj.Id,
                    //SenderAppUserId=candDetail.SenderObj.Id,
                    SenderEmail=candDetail.SenderObj.Email ?? "",
                    RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                    RecipientEmail = candDetail.RecipientObj.Email ?? "",
                    //CCEmail = HRSupobj?.Email ?? "",
                    Subject = subject,
                    Content = msgBody,
                    MessageType = "EmigrationClearanceAdviseByEmail",
                    MessageComposedOn = _today
                };

                var msgs = new List<Message> {message};

                msgWithErr.Messages=msgs;
                
                return msgWithErr;
        }


        public MessageWithError AdviseCandidate_MedicallyFit(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
            var msgWithErr = new MessageWithError();

            var subject = "You have been medically declared Fit";
            var subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                " for " + candDetail.CustomerName + " - you are declared medically fit</u>";
      
            var msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                msgBody += "Pleased to advise you have been declared medically Fit.  Your visa formalities are now proceeding.<br><br>";

                msgBody += "Best Regards<br>HR Supervisor";

                var message = new Message
                {
                    SenderUsername=candDetail.SenderObj.UserName,
                    //RecipientAppUserId=candDetail.RecipientObj.Id,
                    //SenderAppUserId=candDetail.SenderObj.Id,
                    SenderEmail=candDetail.SenderObj.Email ?? "",
                    RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                    RecipientEmail = candDetail.RecipientObj.Email ?? "",
                    //CCEmail = HRSupobj?.Email ?? "",
                    Subject = subject,
                    Content = msgBody,
                    MessageType = "MedicalFitnessAdviseByEmail",
                    MessageComposedOn = _today
                };

                var msgs = new List<Message> {message};

                msgWithErr.Messages=msgs;
                
                return msgWithErr;
        }

        public MessageWithError AdviseCandidate_MedicallyUnfit(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
            var msgWithErr = new MessageWithError();

            var subject = "You are medically Unfit";
            var subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                " for " + candDetail.CustomerName + " - you are Medically Unfit for employment in " + candDetail.CustomerCity + "</u>";
      
            var msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                msgBody += "Regret to inform you the Medical Center that conducted medical fitness tests on you " +
                    "has advised that you are medically Unfit as per GAMMCA rules.  The regulations stipulate that you can " + 
                    "offer yourself for medical fitness tests after 3 months.  However, your proposed employer may not wait till then.<br><br>" +
                    "Please check with the Medical Center reasons for your failing the tests.  Should you be treated for the illness, <br><br>" +
                    "and you continue to be interested in the overseas employment, please do let us know, so that we will keep you on our active list of interested candidates.<br><br>";


                msgBody += "Best Regards<br>HR Supervisor";

                var message = new Message
                {
                    SenderUsername=candDetail.SenderObj.UserName,
                    //RecipientAppUserId=candDetail.RecipientObj.Id,
                    //SenderAppUserId=candDetail.SenderObj.Id,
                    SenderEmail=candDetail.SenderObj.Email ?? "",
                    RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                    RecipientEmail = candDetail.RecipientObj.Email ?? "",
                    //CCEmail = HRSupobj?.Email ?? "",
                    Subject = subject,
                    Content = msgBody,
                    MessageType = "MedicalUnfitnessAdvise",
                    MessageComposedOn = _today
                };

                var msgs = new List<Message> {message};

                msgWithErr.Messages=msgs;
                
                return msgWithErr;
        }

        public async Task<MessageWithError> AdviseCandidate_TicketBooked(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
            var msgWithErr = new MessageWithError();

            var subject = "You have been booked for overseas travel";
            var subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                " for " + candDetail.CustomerName + " - your air ticket for travel to " + candDetail.CustomerCity + "is booked</u>";
      
            var msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

            msgBody += "Please note you have been booked for your overseas travel as follows.  " +
                    "Please contact us to collect your travel documents and for an orientation on working abroad.<br><br>";

            var flight = await _context.CandidateFlights
                .Where(x => x.CvRefId == candDetail.CVRefId).FirstOrDefaultAsync();
            
            if(flight != null) {
                msgBody +=  GetBookingDetails(flight, candDetail.SelectedAs );
                msgBody += "<br><br>";
            }
                msgBody += "Best Regards<br>HR Supervisor";

            var message = new Message
            {
                SenderUsername=candDetail.SenderObj.UserName,
                //RecipientAppUserId=candDetail.RecipientObj.Id,
                //SenderAppUserId=candDetail.SenderObj.Id,
                SenderEmail=candDetail.SenderObj.Email ?? "",
                RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                RecipientEmail = candDetail.RecipientObj.Email ?? "",
                //CCEmail = HRSupobj?.Email ?? "",
                Subject = subject,
                Content = msgBody,
                MessageType = "EmigrationClearanceAdviseByEmail",
                MessageComposedOn = _today
            };

            var msgs = new List<Message> {message};

            msgWithErr.Messages=msgs;
            
            return msgWithErr;
        }

        public MessageWithError AdviseCandidate_VisaIssued(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
             var msgWithErr = new MessageWithError();

            var subject = "Your Visa has been issued/endorsed";
            var subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                " for " + candDetail.CustomerName + " - your Visa is issued/endorsed</u>";
      
            var msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                msgBody += "Pleased to advise your Visa has been issued.  Your travel formalities are under process and we will advise you of the updates.<br><br>";

                msgBody += "Best Regards<br>HR Supervisor";

                var message = new Message
                {
                    SenderUsername=candDetail.SenderObj.UserName,
                    //RecipientAppUserId=candDetail.RecipientObj.Id,
                    //SenderAppUserId=candDetail.SenderObj.Id,
                    SenderEmail=candDetail.SenderObj.Email ?? "",
                    RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                    RecipientEmail = candDetail.RecipientObj.Email ?? "",
                    //CCEmail = HRSupobj?.Email ?? "",
                    Subject = subject,
                    Content = msgBody,
                    MessageType = "VisaIssueAdviseByMail",
                    MessageComposedOn = _today
                };

                var msgs = new List<Message> {message};

                msgWithErr.Messages=msgs;
                
                return msgWithErr;
        }

        public MessageWithError AdviseCandidate_VisaRejected(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
             var msgWithErr = new MessageWithError();

            var subject = "Your Visa has been rejected";
            var subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                " for " + candDetail.CustomerName + " - your Visa is rejected</u>";
      
            var msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                msgBody += "Regret to advise you your Visa has been rejected by the Consulate.  Therefore, your employment with the foreign country " +
                    " cannot take effect.  If you want, we will continue with our efforts to find you an alternate employment opportunity.<br><br>";

                msgBody += "Best Regards<br>HR Supervisor";

                var message = new Message
                {
                    SenderUsername=candDetail.SenderObj.UserName,
                    //RecipientAppUserId=candDetail.RecipientObj.Id,
                    //SenderAppUserId=candDetail.SenderObj.Id,
                    SenderEmail=candDetail.SenderObj.Email ?? "",
                    RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                    RecipientEmail = candDetail.RecipientObj.Email ?? "",
                    //CCEmail = HRSupobj?.Email ?? "",
                    Subject = subject,
                    Content = msgBody,
                    MessageType = "VisaRejectionAdviseByMail",
                    MessageComposedOn = _today
                };

                var msgs = new List<Message> {message};

                msgWithErr.Messages=msgs;
                
                return msgWithErr;
        }
    
        private static string GetBookingDetails(CandidateFlight Flight, string SelectedAs )
        {
            string FlightDetail = "<b>Candidate:</b>:" + Flight.ApplicationNo + "-" + Flight.CandidateName;
            FlightDetail += "<br><b>Selected For</b>:" + Flight.CustomerName + ", " + Flight.CustomerCity;
            FlightDetail += "<br><b>Selected As</b>:" + SelectedAs;
            FlightDetail += "<br><br>Date of Flight</b>: " + Flight.ETD_Boarding.ToString("dd-MMM-yy hh:nn");
            FlightDetail += "<br><b>Airport of Boarding</b>: " + Flight.AirportOfBoarding;
            FlightDetail += "<br><b>Airport of Destination</b>: " + Flight.AirportOfDestination;
            FlightDetail += "<br><b>Flight No.</b>: " + Flight.FlightNo;
            
            if(!string.IsNullOrEmpty(Flight.AirportVia)) {
                FlightDetail += "<br><b>Via</b>: " + Flight.AirportVia + "(ETA At" + Flight.ETA_Via + ", ETD From " 
                + Flight.ETD_Via + ")";
            };
            
            return FlightDetail;
        }

        

        public MessageWithError AdviseCandidate_OfferAccepted(CandidateAdviseDto candDetail, DateTime TransactionDate)
        {
            throw new NotImplementedException();
        }
    }
}