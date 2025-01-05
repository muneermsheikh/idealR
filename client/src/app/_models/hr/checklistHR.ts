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
     salaryOffered: string;
     salaryExpectation: number;
     checklistedOk: boolean;
     
     checklistHRItems: IChecklistHRItem[];
}