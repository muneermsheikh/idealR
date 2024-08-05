export interface ITaskItem {
     id: number;
     appTaskId: number;
     transactionDate: Date;
     taskStatus: string;
     taskItemDescription: string;
     userName: string;
     nextFollowupOn: Date;
     nextFollowupByName: string;
}