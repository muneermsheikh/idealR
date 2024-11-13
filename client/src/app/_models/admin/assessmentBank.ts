import { IAssessmentBankQ } from "./assessmentBankQ";

export interface IAssessmentBank
{
    id: number;
    professionId: number;
    professionName: string;
    assessmentBankQs: IAssessmentBankQ[];
}