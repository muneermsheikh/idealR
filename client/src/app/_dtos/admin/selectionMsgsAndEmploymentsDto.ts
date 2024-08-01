
import { IMessage } from "src/app/_models/admin/message";
import { IEmploymentDto } from "./employmentDto";

export interface ISelectionMsgsAndEmploymentsDto
{
     emailMessages: IMessage[];
     //employmentDtos: IEmploymentDto[];
     cvRefIdsAffected: number[];
}