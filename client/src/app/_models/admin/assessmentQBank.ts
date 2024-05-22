import { IAssessmentStandardQ } from "./assessmentStandardQ";

export interface IAssessmentQBank
{
     id: number;
     professionId: number;
     professionName: string;
     assessmentStddQs: IAssessmentStandardQ[];
}
export class AssessmentQBank
{
     id=0;
     professionId=0;
     professionName='';
     assessmentStddQs=null;
}

export interface IAssessmentQBankItem
{
     id: number;
     assessmentQBankId: number;
     assessmentParameter: string;
     qNo: number;
     isStandardQ: boolean;
     question: string;
     maxPoints: number;
}