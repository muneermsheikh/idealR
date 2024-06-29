import { IEmployment } from "./employment";

export interface ISelectionDecision
{
     checked: boolean;
     id: number;
     cvRefId: number;
     orderItemId: number;
     professionId: number;
     professionName: string;
     //orderNo: number;
     customerName: string;
     cityOfWorking: string;
     charges: number;
     applicationNo: number;
     candidateId: number;
     candidateName: string;
     selectedOn: Date;
     selectionStatus: string;
     //employment: IEmployment;
     remarks: string;
}

