import { IChecklistHRItem } from "./checklistHRItem";

export interface IChecklistHR{

     id: number;
     candidateId: number;
     orderItemId: number;
     userName: string;
     checkedOn: Date;
     userComments: string;
     hRExecUsername: string;
     charges: number;
     chargesAgreed: number;
     exceptionApproved: boolean;
     exceptionApprovedOn: Date;
     exceptionApprovedBy: string;
     checklistedOk: boolean;
     
     checklistHRItems: IChecklistHRItem[];
}