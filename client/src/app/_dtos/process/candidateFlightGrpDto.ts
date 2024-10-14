
export interface ICandidateFlightGrpDto
{
    id: number;
    dateOfFlight: Date;
    orderNo: number;

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
    countOfPax: number;
}

export class CandidateFlightGrpDto implements ICandidateFlightGrpDto
{
    id=0;
    dateOfFlight=new Date;
    orderNo=0;
    
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
    countOfPax: number=0;
}