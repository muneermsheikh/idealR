import { Time } from "@angular/common";
import { StringNullableChain } from "lodash";

export interface IFlightdata
{
    //id: number;
    airlineName: string;
    flightNo: string;
    airportOfBoarding: string;
    airportOfDestination: string;
    airportVia: string;
    flightNoVia: string;
    airlineVia: string;
    eTD_Boarding: Date;
    eTD_BoardingString: string;
    eTA_Destination: Date;
    eTA_DestinationString: string;
    eTA_Via: Date|null;
    eTD_Via: Date | null;
    eTA_ViaString: string;
    eTD_ViaString: string;
}

export class Flightdata implements IFlightdata
{
    //id=0;
    airlineName='';
    flightNo='';
    airportOfBoarding = '';
    airportOfDestination='';
    airportVia='';
    flightNoVia='';
    airlineVia='';
    eTD_Boarding= new Date;
    eTA_Destination= new Date;
    eTD_BoardingString = '';
    eTA_DestinationString = '';
    eTA_Via= new Date;
    eTD_Via= new Date;
    eTA_ViaString='';
    eTD_ViaString= '';
}