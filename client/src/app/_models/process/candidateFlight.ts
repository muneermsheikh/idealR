

export interface ICandidateFlight
{
    id: number;
    depId: number;
    depItemId: number;
    cvRefId: number;
    applicationNo: number;
    candidateName: string;
    customerName: string;
    customerCity: string;

    airlineName: string;
    flightNo: string;
    airportOfBoarding: string;
    airportOfDestination: string;
    eTD_Boarding: Date;
    eTA_Destination: Date;
    airportVia: string;
    flightNoVia: string;
    eTA_Via?: Date | null;
    eTD_Via?: Date | null;
    fullPath: string;

    fileToUpload: File|null;

}

export class CandidateFlight implements ICandidateFlight
{
    id=0;
    depId = 0;
    depItemId=0;
    cvRefId=0;
    applicationNo=0;
    candidateName='';
    customerName='';
    customerCity='';

    airlineName = '';
    flightNo = ''; 
    airportOfBoarding = '';
    flightNoVia = ''; 
    airportOfDestination = '';
    eTD_Boarding= new Date;
    eTA_Destination= new Date;
    airportVia = '';
    eTD_Via= null;    //new Date;
    eTA_Via= null;  // new Date;
    fullPath='';
    fileToUpload= null;
}