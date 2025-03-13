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
    etD_Boarding: Date;
    etA_Destination: Date;
    etD_BoardingString: string;
    etA_DestinationString: string;
    
    airportVia: string;
    flightNoVia: string;
    etA_Via?: Date | null;
    etD_Via?: Date | null;
    etA_ViaString: string;
    etD_ViaString: string;
    
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
    etD_Boarding= new Date;
    etA_Destination= new Date;
    etA_DestinationString='';
    etD_BoardingString='';
    airportVia = '';
    etD_Via= null;    //new Date;
    etA_Via= null;  // new Date;
    etD_ViaString='';
    etA_ViaString= '';
    
    fullPath='';
    fileToUpload= null;
    candidateFlightItems:ICandiateFlightItem[]=[];
}