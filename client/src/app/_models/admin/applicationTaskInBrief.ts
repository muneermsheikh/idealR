export interface IApplicationTaskInBrief {
     id: number;
     taskType: string;

     taskDate: Date;
     assignedByUsername: string;
     assignedToUsername: string;
     
     orderNo: number;
     orderItemId: number;
     applicationNo: number;
     taskDescription: string;
     completeBy: Date;
     taskStatus: string;
     
}

export class ApplicationTaskInBrief implements IApplicationTaskInBrief {
     id=0;
     taskType="";
     taskDate=new Date();
     assignedByUsername="";
     assignedToUsername="";
     orderNo=0;
     orderItemId=0;
     applicationNo=0;
     taskDescription="";
     completeBy=new Date();
     taskStatus="";

}