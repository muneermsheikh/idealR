import { IRemuneration } from "src/app/_models/admin/remuneration";

export interface IRemunerationDto
{
     id: number;
     customerName: string;
     categoryName: string;
     orderDate: Date;
     orderItemId: number; 
     orderId: number; 
     orderNo: number; 
     professionId: number; 

     workHours: number;
     salaryCurrency: string; 
     salaryMin: number; 
     salaryMax: number; 
     contractPeriodInMonths: number;
     housingProvidedFree: boolean; 
     housingAllowance: number; 
     housingNotProvided: boolean;
     foodProvidedFree: boolean; 
     foodAllowance: number; 
     foodNotProvided: boolean;
     transportProvidedFree: boolean; 
     transportAllowance: number; 
     transportNotProvided: boolean;
     otherAllowance: number; 
     leavePerYearInDays: number; 
     leaveAirfareEntitlementAfterMonths: number;
     
}