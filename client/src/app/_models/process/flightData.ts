import { Time } from "@angular/common";

export interface IFlightdata
{
    id: number;
    checked: boolean;
    flightNo: string;
    airportOfBoarding: string;
    airportOfDestination: string;
    airportVia: string;
    flightNoVia: string;
    eTD_Boarding: Time;
    eTA_Destination: Time;
    eTA_Via?: Time;
    eTD_Via?: Time;

}