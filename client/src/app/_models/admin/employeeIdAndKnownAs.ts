import { IHRSkill } from "./hrSkill";

export interface IEmployeeIdAndKnownAs
{
     id: number;
     knownAs: string;
     username: string;
     hrSkills: IHRSkill[];
}