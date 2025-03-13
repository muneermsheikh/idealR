export class visaParams
{
    id : number=0;
    visaItemId: number = 0;
    visaNo: string = '';
    customerId: number = 0;
    customerName: string = '';
    cvRefId: number = 0;
    visaExpiryH: string = '';
    visaExpiryG: Date = new Date(1900,1,1);
    visaSponsorName: string = '';
    
    visaCategory: string='';
    visaConsulate: string='';
    visaCanceled: boolean = false;

    depItemId: number = 0;
    visaApproved: boolean = false;

    sort = "visano";
    pageNumber = 1;
    pageSize = 15;
    search: string='';
}