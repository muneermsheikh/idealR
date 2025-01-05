export interface IIndustryType {
     id: number;
     industryName: string;
     industryGroup: string;   //type f industry, O&G, Construction, O&M, etc.
     industryClass: string;   //manufacturing, O&M, consultancy, etc.
}

export class IndustryType implements IIndustryType {
     id: number = 0;
     industryName: string = '';
     industryGroup: string = '';   //type f industry, O&G, Construction, O&M, etc.
     industryClass: string = '';   //manufacturing, O&M, consultancy, etc.
}