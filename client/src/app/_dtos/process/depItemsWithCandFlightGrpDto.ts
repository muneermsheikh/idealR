import { ICandidateFlightGrp } from "src/app/_models/process/candidateFlightGrp";
import { IDepItemToAddDto } from "./depItemToAddDto";

export class DepItemsWithCandFlightGrpDto
{
    depItemsToAdd: IDepItemToAddDto[]=[];
    candFlightGrpWithItems: ICandidateFlightGrp|undefined;
}