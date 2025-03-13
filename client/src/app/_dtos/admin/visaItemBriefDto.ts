export interface IVisaItemBriefDto {
    checked: boolean;
    customerKnownAs: string;
    visaNo: string;
    visaItemId: number;
    visaDateG: Date;
    visaDateH: string;
    visaExpiryG: Date;
    visaExpiryH: string;
    visaCategoryInEnglish: string;
    visaCategoryInArabic: string;
    visaQuantity: number;
    visaBalance: number;
}