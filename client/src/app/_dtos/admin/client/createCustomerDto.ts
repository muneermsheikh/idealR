import { ICustomerOfficialToCreateDto } from "../customerOfficialToCreateDto";

export interface CreateCustomerDto
{
    id: number;
    customerType: string;
    customerName: string;
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
    customerOfficials: ICustomerOfficialToCreateDto[];
}