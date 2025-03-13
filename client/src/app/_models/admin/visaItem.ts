export interface IVisaItem
{
    id: number;
    srNo: number;
    visaHeaderId: number;
    visaNo: string;
    visaCategoryArabic: string;
    visaCategoryEnglish: string;
    visaConsulate: string;
    visaQuantity: string;
    visaCanceled: boolean;
}

export class VisaItem implements IVisaItem
{
    id = 0;
    srNo = 0;
    visaHeaderId = 0;
    visaNo='';
    visaCategoryArabic = '';
    visaCategoryEnglish = '';
    visaConsulate= '';
    visaQuantity = '';
    visaCanceled = false;
}