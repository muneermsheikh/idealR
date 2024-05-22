export interface IAssessmentQBankDto
{
    id: number;
    professionId: number;
    professionName: string;
    assessmentParameter: string;
    qNo: number;
    isStandardQ: boolean;
    question: string;
    maxPoints: number;

}