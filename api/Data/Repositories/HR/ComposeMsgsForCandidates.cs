using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Interfaces.HR;
using api.Interfaces.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.HR
{
    public class ComposeMsgsForCandidates : IComposeMsgsForCandidates
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly DateTime _today = DateTime.Now;
        private readonly IConfiguration _config;
        private readonly string _RAName;
        

        public ComposeMsgsForCandidates(DataContext context, UserManager<AppUser> userManager, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
            _context = context;
            _RAName = _config["_RAName"] ?? "";
        }
        public async Task<MessageWithError> AdviseCandidate_DeploymentStatus(CandidateAdviseDto candDetail, DateTime TransactionDate, string ProcessName)
        {
            var msgWithErr = new MessageWithError();

            var subject = "";
            var subjectInBody = "";
            var msgBody = "";
            var msgType = "";
            var appUserIdHRSup = _config["HRSupAppuserId"] ?? "0";
            var senderObj = await _userManager.FindByIdAsync(appUserIdHRSup);
            
            var candidateName = candDetail.CandidateTitle + " " + candDetail.CandidateName;

            var Notification = new FCNMessage();

            switch (ProcessName.ToLower()) {
                
                case "emigrationcleared":
                    subject = "Your Emigration Clearance is received";
                    subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                        " for " + candDetail.CustomerName + " - your Emigration Clearance is received</u>";

                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += "Pleased to advise the Emigration Authorities have cleared your emigration.  " +
                        "As you are aware, your Passport requires Emigration Clearance, " + 
                        "and this procedure was required before you could travel abroad for employment.<br><br>" +
                        "We are now booking your air ticket for your overseas travel, and will advise you once it is booked.";
                    
                    //FCM Push Notifications
                    Notification = new FCNMessage{
                        Topic = "Notification - Emigration Cleared",
                        Notification = {Title = "Your Emigration Cleared",
                            Body = candidateName  + " - Your emigration clarance has been received. " +
                            "Please check the email sent to you. Regards/" + _RAName + ", Processing Division "},
                        Android = new AndroidConfig ()
                    };

                    break;
                
                case "medicallyfit":
                    subject = "You have been medically declared Fit";
                    subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                    " for " + candDetail.CustomerName + " - you are declared medically fit</u>";
        
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                    candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                            "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                            candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += "Pleased to advise you have been declared medically Fit.  Your visa formalities are now proceeding.";
                    msgType = "DeploymentProcess";   //"MedicalFitnessAdviseByEmail";

                    //FCM Push Notifications
                    Notification = new FCNMessage{
                        Topic = "Notification - Medically Fit",
                        Notification = {Title = "You are medically declared FIT",
                            Body = candidateName  + " - You are declared medically Fit. Your Visa Processing is initiated.  " +
                            "Please check the email sent to you on this subject. Regards/" + _RAName + ", Processing Division "},
                        Android = new AndroidConfig ()
                    };

                    break;

                case "medicallyunfit":
                    subject = "You are medically Unfit";
                    subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                        " for " + candDetail.CustomerName + " - you are Medically Unfit for employment in " + candDetail.CustomerCity + "</u>";
        
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                    candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                            "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                            candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += "Regret to inform you the Medical Center that conducted medical fitness tests on you " +
                        "has advised that you are medically Unfit as per GAMMCA rules.  The regulations stipulate that you can " + 
                        "offer yourself for medical fitness tests again after 3 months.  However, your proposed employer may not wait till then.<br><br>" +
                        "Please check with the Medical Center reasons for your failing the tests.  Should you be treated for the illness, <br><br>" +
                        "and you continue to be interested in the overseas employment, please do let us know, so that we will keep you on our active list of interested candidates.";

                    msgType = "DeploymentProgress";     // "MedicalUnfitnessAdvise";

                    //FCM Push Notifications
                    Notification  = new FCNMessage{
                        Topic = "Notification - Medicaly Unfit",
                        Notification = {Title = "You are medically Unfit",
                            Body = candidateName  + " - You are declared medically unfit.  It ceases all further deployment procesemigration clarance has been received. " +
                            "Please check the email sent to you. Regards/" + _RAName + ", Processing Division "},
                        Android = new AndroidConfig ()
                    };

                    break;
                
                case "visaissued":
                    subject = "Your Visa has been issued/endorsed";
                    subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                        " for " + candDetail.CustomerName + " - your Visa is issued/endorsed</u>";
      
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.RecipientObj.Email ?? "" + "<br><br>" + 
                        "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += "Pleased to advise your Visa has been issued.  Your travel formalities are under process and we will advise you of the updates.";
                    msgType = "DeploymentProgress"; // "VisaIssueAdviseByMail";

                    //FCM Push Notifications
                    Notification  = new FCNMessage{
                        Topic = "Notification - Visa Issued",
                        Notification = {Title = "Your Visa is issued",
                            Body = candidateName  + " - Your Visa is issued.  Next Process is Emigration Clearance. " +
                            "Please check the email sent to you. Regards/" + _RAName + ", Processing Division "},
                        Android = new AndroidConfig ()
                    };

                    break;
                
                case "visarejected":
                    subject = "Your Visa has been rejected";
                    subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                        " for " + candDetail.CustomerName + " - your Visa is rejected</u>";
      
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                    candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.CandidateEmail ?? "" + "<br><br>" + 
                        "copy: " + candDetail.SenderObj.Email ?? ""  + "<br><br>Dear " + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += "Regret to advise you your Visa has been rejected by the Consulate.  Therefore, your employment with the foreign country " +
                        " cannot take effect.  If you want, we will continue with our efforts to find you an alternate employment opportunity.";
                    msgType = "DeploymentProgress"; // "VisaRejectionAdviseByMail";

                    //FCM Push Notifications
                    Notification  = new FCNMessage{
                        Topic = "Notification - Visa Rejected",
                        Notification = {Title = "Your visa has been rejected",
                            Body = candidateName  + " - Your Visa is Rejected.  Please discuss with our Office or visit in person " +
                            "if you can visit, to understand reasons for the rejection and if it can be resolved.  " +
                            "Please read the email message sent to you.  Regards/Processing Department"},
                        Android = new AndroidConfig ()
                    };

                    break;
                
                case "ticketbooked":

                    subject = "You have been booked for overseas travel";
                    subjectInBody = "<b><u>Subject: </b>Your Selection as " + candDetail.SelectedAs +
                        " for " + candDetail.CustomerName + " - your air ticket for travel to " + candDetail.CustomerCity + "is booked</u>";
            
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                        candDetail.CandidateTitle + " " + candDetail.CandidateName + "email: " + candDetail.RecipientObj.Email ?? "" + "<br><br>" + 
                                "<br><br>Dear " + 
                                candDetail.CandidateTitle + " " + candDetail.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += "Please note you have been booked for your overseas travel as follows.  " +
                            "Please contact us to collect your travel documents and for an orientation on working abroad.<br><br>";
                    msgType =  "DeploymentProgress"; //"EmigrationClearanceAdviseByEmail";

                    var flight = await _context.CandidateFlights
                        .Where(x => x.CvRefId == candDetail.CVRefId).FirstOrDefaultAsync();
                    
                    if(flight != null) msgBody +=  GetBookingDetails(flight, candDetail.SelectedAs);

                    //FCM Push Notifications
                    Notification  = new FCNMessage{
                        Topic = "Notification - Your travel is booked",
                        Notification = {Title = "Your air travel is booked",
                            Body = candidateName  + " - Your travel plan is finalized.  Please check your email for travel details. "},
                        Android = new AndroidConfig ()
                    };

                    break;

                default:
                    break;

            }
            
            msgBody += "<br><br>Best Regards<br>HR Supervisor";

            var message = new Message
            {
                SenderUsername=senderObj.UserName,
                //RecipientAppUserId=candDetail.RecipientObj.Id,
                //SenderAppUserId=candDetail.SenderObj.Id,
                SenderEmail=senderObj.Email ?? "",
                RecipientUsername = candDetail.RecipientObj.UserName ?? "",
                RecipientEmail = candDetail.RecipientObj.Email ?? "",
                //CCEmail = HRSupobj?.Email ?? "",
                Subject = subject,
                Content = msgBody,
                MessageType = msgType,
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