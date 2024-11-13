export interface IAssessmentBankQ
{
    id: number;
    assessmentBankId: number;
    assessmentParameter: string;
    qNo: number;
    isStandardQ: boolean;
    isMandatory: boolean;
    question: string;
    maxPoints: number;
}