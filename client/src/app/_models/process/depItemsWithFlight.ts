import { IDepItem } from "./depItem";
import { ICandidateFlight } from "./candidateFlight";

export interface IDepItemWithFlightDto
{
    depItem?: IDepItem;
    candidateFlight?: ICandidateFlight;
}

export class depItemWithFlight implements IDepItemWithFlightDto
{
    depItem?:IDepItem;
    candidateFlight? : ICandidateFlight;
}
