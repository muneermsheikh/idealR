
import { ICustomerIndustry } from "src/app/_models/admin/customerIndustry";
import { ICustomerOfficialToCreateDto } from "./customerOfficialToCreateDto";
import { IAgencySpecialty } from "src/app/_models/admin/agencySpecialty";

export interface IRegisterCustomerDto
{
     id: number;
     customerName: string;
     customerType: string;
     knownAs: string;
     add: string;
     add2: string;
     city: string;
     pin: string;
     district: string;
     state: string;
     country: string;
     email: string;
     website: string;
     phone: string;
     phone2: string;
     introduction: string;
     customerIndustries: ICustomerIndustry[];
     agencySpecialties: IAgencySpecialty[];
     customerOfficials: ICustomerOfficialToCreateDto[];
}

