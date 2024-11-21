import { ITaskItem } from "./taskItem";

export interface IApplicationTask {
     id: number;
     taskType: string;
     taskDate: Date;
     assignedToUsername: string;
     assignedByUsername: string;
     taskDescription: string;
     completeBy: Date;
     taskStatus: string;
     completedOn: Date;
     postTaskAction: number;
     historyItemId: number;

     orderId?: number;
     orderNo?: number;
     orderItemId?: number;
     qntyAssigned: number;
     qntyDelivered: number;
     applicationNo?: number;
     candidateId?: number;
     resumeId: string;
     taskItems: ITaskItem[];
}
export class ApplicationTask implements IApplicationTask {
     id= 0;
     taskType= '';
     taskDate= new Date();
     assignedToUsername='';
     assignedByUsername='';
     taskDescription= '';
     completeBy=new Date();
     taskStatus= '';
     completedOn= new Date();
     postTaskAction= 0;
     historyItemId= 0;

     orderId= 0;
     orderNo= 0;
     orderItemId= 0;
     qntyAssigned = 0;
     qntyDelivered = 0;
     applicationNo= 0;
     candidateId= 0;
     resumeId= '';
     taskItems: ITaskItem[]=[];
}
