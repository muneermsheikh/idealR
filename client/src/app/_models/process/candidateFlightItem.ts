export interface ICandiateFlightItem
{
    id: number;
    depId: number;
    depItemId: number;
    cvRefId: number;
    applicationNo: number;
    candidateName: string;
    categoryName: string;
    customerName: string;
    customerCity: string;
}

export class CandidateFlightItem implements ICandiateFlightItem
{
    id = 0;
    depId = 0;
    depItemId=0;
    cvRefId=0;
    applicationNo=0;
    candidateName='';
    customerName='';
    categoryName='';
    customerCity='';

}