import { IDeploymentDto } from "src/app/_models/process/deploymentdto";


export interface IDeploymentDtoWithErrorDto
{
	deploymentObjDtos: IDeploymentDto[];
	errorStrings: string[];
}