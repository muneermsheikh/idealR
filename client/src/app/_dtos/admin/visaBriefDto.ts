export interface IVisaBriefDto
{
    id: number;
    visaItemId: number;
    customerKnownAs: string;
    customerId: number;
    visaNo: string;
    visaConsulate: string;
    visaDateH: string;
    visaDateG: Date;
    visaCategoryEnglish: string;
    visaSponsorName: string;
    visaQuantity: number;
    visaConsumed: number;
    visaBalance: number;
    depItemId: number;
    visaCanceled: boolean;
}