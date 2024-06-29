export interface ICandidateFlightData
{
    depId: number;
    dateOfFlight: Date; 
    flightNo: string; 
    airportOfBoarding: string;
    flightNoVia: string; 
    airportOfDestination: string;
    eTD_Boarding: Date;
    eTA_Destination: Date;
    airportVia: string;
    eTD_Via?: Date;
    eTA_Via?: Date;
}