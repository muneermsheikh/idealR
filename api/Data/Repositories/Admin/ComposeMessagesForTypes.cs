using System.Threading.Tasks;
using api.DTOs.Admin;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Extensions;
using api.Interfaces.Admin;

namespace api.Data.Repositories.Admin
{
    public class ComposeMessagesForTypes: IComposeMessagesForTypes
    {
        private readonly DataContext _context;
        public ComposeMessagesForTypes(DataContext context)
        {
            _context = context;
        }


        public Task<string> AssessmentGrade(int candidateId, int orderitemId)
        {
            throw new NotImplementedException();
        }

        //
        public async Task<string> ComposeOrderItems(int orderNo, ICollection<OrderItem> orderItems, bool hasException)
        {
            var personnel = "";
            
            personnel = "<Table><th width=50>Reference</th><th width=250>Category</th><th width=50>Quantity</th>";
            foreach(var item in orderItems) {
                personnel += "<td>" + orderNo + "-" + item.SrNo + "</td>" + 
                    "<td>" +  item.Profession?.ProfessionName ?? 
                        await _context.GetProfessionNameFromId(item.ProfessionId) + "</td>" +
                    "<td>" + item.Quantity + "</td>";
            }

            personnel +="</Table>";

            return personnel;
        }

        public Task<OrderItemReviewStatusDto> CumulativeCountForwardedSoFar(int orderitemId)
        {
            throw new NotImplementedException();
        }

        public string GetSelectionDetails(string CandidateName, int ApplicationNo, string CustomerName, string CategoryName, Employment employmt)
        {
            string strToReturn = "";
            strToReturn = "<ul><li><b>Employee Name:</b> " + CandidateName + "(Application No.:" + ApplicationNo + ")</li>" +
                    "<li><b>Employer</b>: " + CustomerName + "</li>" +
                    "<li><b>Selected as:</b> " + CategoryName + 
                    "<li><b>Contract Period:</b>" + employmt.ContractPeriodInMonths + " months</li>" +
                    "<li><b>Basic Salary:</b>" + employmt.SalaryCurrency + " " + employmt.Salary + "</li>" +
                    "<li><b>Housing Provision: </b>";
                    if (employmt.HousingProvidedFree) { strToReturn += "Provided Free"; }
                    else { strToReturn += employmt.HousingAllowance > 0 
                        ? employmt.SalaryCurrency + " " + employmt.HousingAllowance : "Not provided"; }
            strToReturn += "</li>" +
                    "<li><b>Food Provision:</b>";
                    if (employmt.FoodProvidedFree) { strToReturn += "Provided Free"; }
                    else {strToReturn += employmt.FoodAllowance > 0 ? 
                        employmt.SalaryCurrency + " " + employmt.FoodAllowance : "Not Provided"; }
            strToReturn += "</li>" +
                    "<b><li>Transport Provision:</b> ";
                    if (employmt.TransportProvidedFree) { strToReturn += "Provided Free"; }
                    else { strToReturn += employmt.TransportAllowance > 0 
                        ? employmt.SalaryCurrency + " " + employmt.TransportAllowance : "Not provided"; }
            strToReturn += "</li>";
            if (employmt.OtherAllowance > 0) strToReturn += "<li><b>Other Allowances:</b>" + employmt.SalaryCurrency + " " + employmt.OtherAllowance + "</li>";
            return strToReturn + "</ul>";
        }

        public string GetSelectionDetailsBySMS(SelectionDecision selection, string customerName, string professionName)
        {
            string strToReturn = "";
            strToReturn = "Pleased to advise you hv been selected by " + customerName + " as " + professionName;
            //strToReturn += " at a basic salary of " + selection.Employment!.SalaryCurrency + " " + selection.Employment.FoodAllowance;
            strToReturn += " plus perks.  Please visit us to review and sign your offer letter and to initiate your joining formalities";
            return strToReturn;
        }

        public string GetSelectionDetailsBySMS(SelectionDecision selection)
        {
            throw new NotImplementedException();
        }

        public Task<string> TableOfOrderItemsContractReviewedAndApproved(ICollection<int> itemIds)
        {
            throw new NotImplementedException();
        }

        public Task<string> TableOfRelevantOpenings(List<int> Ids)
        {
            throw new NotImplementedException();
        }

    }
}