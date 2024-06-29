

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
    dateOfFlight: Date;

    flightNo: string;
    airportOfBoarding: string;
    airportOfDestination: string;
    eTD_Boarding: Date;
    eTA_Destination: Date;
    airportVia: string;
    flightNoVia: string;
    eTA_Via?: Date;
    eTD_Via?: Date;

}