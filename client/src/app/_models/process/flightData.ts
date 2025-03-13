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
    etD_Boarding: Date;
    etD_BoardingString: string;
    etA_Destination: Date;
    etA_DestinationString: string;
    etA_Via: Date|null;
    etD_Via: Date | null;
    etA_ViaString: string;
    etD_ViaString: string;
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
    etD_Boarding= new Date;
    etA_Destination= new Date;
    etD_BoardingString = '';
    etA_DestinationString = '';
    etA_Via= new Date;
    etD_Via= new Date;
    etA_ViaString='';
    etD_ViaString= '';
}