import { ICandiateFlightItem } from "./candidateFlightItem";


export interface ICandidateFlightGrp
{
    id: number;
    dateOfFlight: Date;
    orderNo: number;
    customerId: number;

    airlineName: string;
    flightNo: string;
    airportOfBoarding: string;
    airportOfDestination: string;
    eTD_Boarding: Date;
    eTA_Destination: Date;
    eTD_BoardingString: string;
    eTA_DestinationString: string;
    
    airportVia: string;
    flightNoVia: string;
    eTA_Via?: Date | null;
    eTD_Via?: Date | null;
    eTA_ViaString: string;
    eTD_ViaString: string;
    
    fullPath: string;

    fileToUpload: File|null;
    candidateFlightItems: ICandiateFlightItem[];

}

export class CandidateFlightGrp implements ICandidateFlightGrp
{
    id=0;
    dateOfFlight=new Date;
    orderNo=0;
    customerId=0;
    
    airlineName = '';   
    flightNo = ''; 
    airportOfBoarding = '';
    flightNoVia = ''; 
    airportOfDestination = '';
    eTD_Boarding= new Date;
    eTA_Destination= new Date;
    eTA_DestinationString='';
    eTD_BoardingString='';
    airportVia = '';
    eTD_Via= null;    //new Date;
    eTA_Via= null;  // new Date;
    eTD_ViaString='';
    eTA_ViaString= '';
    
    fullPath='';
    fileToUpload= null;
    candidateFlightItems:ICandiateFlightItem[]=[];
}