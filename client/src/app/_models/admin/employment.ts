export interface IEmployment
{
	id: number;
	cvRefId: number;
	selectionDecisionId: number;

	selectedOn: Date;
	charges: number;
	salaryCurrency: string;
	salary: number;
	contractPeriodInMonths: number;
	weeklyHours: number;
	housingProvidedFree: boolean;
	housingNotProvided: boolean;
	housingAllowance: number;
	foodProvidedFree: boolean;
	foodNotProvided: boolean;
	foodAllowance: number;
	transportProvidedFree: boolean;
	transportNotProvided: boolean;
	transportAllowance: number;
	otherAllowance: number;
	leavePerYearInDays: number;
	leaveAirfareEntitlementAfterMonths: number;
	offerAccepted: string;

	categoryId: number;
	categoryName: string;
	orderItemId: number;
	orderId: number;
	orderNo: number;
	customerId: number;
	customerName: string;
	candidateId: number;
	applicationNo: number;
	candidateName: string;
	agentName: string;
	approved: boolean;
	approvedByEmpId:number;
	offerAcceptedOn: Date;
	offerAttachmentFileName: string;
	offerAttachmentFullPath: string;

}

export class Employment {
	id= 0;
	cvRefId= 0;
	selectionDecisionId= 0;
	selectedOn= new Date();
	charges= 0;
	salaryCurrency= '';
	salary= 0;
	contractPeriodInMonths= 0;
	weeklyHours= 0;
	housingProvidedFree= false;
	housingNotProvided= false;
	housingAllowance= 0;
	foodProvidedFree= false;
	foodNotProvided= false;
	foodAllowance= 0;
	transportProvidedFree= false;
	transportNotProvided= false;
	transportAllowance= 0;
	otherAllowance= 0;
	leavePerYearInDays= 0;
	leaveAirfareEntitlementAfterMonths= 0;
	offerAccepted= '';
	offerAcceptedOn= new Date();
	categoryId= 0;
	categoryName= '';
	orderItemId= 0;
	orderId= 0;
	orderNo= 0;
	customerId= 0;
	customerName= '';
	candidateId= 0;
	applicationNo= 0;
	candidateName= '';
	agentName= '';
	approved= false;
	approvedByEmpId = 0;
	offerAcceptanceConcludedOn= new Date();

}