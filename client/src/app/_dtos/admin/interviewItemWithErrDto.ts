import { IIntervwItem } from "src/app/_models/hr/intervwItem";

export interface IInterviewItemWithErrDto
{
    intervwItem: IIntervwItem;
    error: string;
}
