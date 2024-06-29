export interface IEmploymentDto
{
     applicationNo: number;
     candidateName: string;
     companyName: string;
     categoryRef: string;

     selectionDecisionId: number;
     cvRefId: number;
     selectedOn: Date;
     charges: number;
     weeklyHours: number;
     salaryCurrency: string;
     salary: number;
     contractPeriodInMonths: number;
     housingProvidedFree: boolean;
     housingAllowance: number;
     housingNotProvided: boolean;
     foodProvidedFree: boolean;
     foodNotProvided: boolean;
     foodAllowance: number;
     transportProvidedFree: boolean;
     transportAllowance: number;
     transportNotProvided: boolean;
     otherAllowance: number;
     leavePerYearInDays: number;
     leaveAirfareEntitlementAfterMonths: number;
     offerAccepted: string;
     offerAcceptedOn: Date;
     remarks: string;
}
