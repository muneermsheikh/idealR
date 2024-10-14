import { ICandidateFlight } from "src/app/_models/process/candidateFlight";
import { IDepItemToAddDto } from "./depItemToAddDto";
import { ICandidateFlightGrp } from "src/app/_models/process/candidateFlightGrp";

export interface IDepItemsAndCandFlightsToAddDto {
    depItemsToAdd: IDepItemToAddDto[],
    candFlightToAdd: ICandidateFlightGrp
}