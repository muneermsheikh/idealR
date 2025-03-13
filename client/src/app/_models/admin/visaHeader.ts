import { IVisaItem, VisaItem } from "./visaItem";

export interface IVisaHeader
{
    id: number;
    visaNo: string;
    customerId: number;
    customerName: string;
    visaDateG: Date;
    visaDateH: string;
    visaExpiryH: string;
    visaExpiryG: Date;
    visaSponsorName: string;
    visaItems: IVisaItem[];
}

export class VisaHeader implements IVisaHeader
{
    id = 0;
    visaNo = '';
    customerId = 0;
    customerName = '';
    visaDateG =  new Date();
    visaDateH = '';
    visaExpiryH = '';
    visaExpiryG = new Date();
    visaSponsorName = '';
    visaItems:IVisaItem[]= [];
}