export class TaskParams {
     taskId=0;
     taskType = "";
     taskDate = new Date('1900-01-01');
     candidateId: number=0;
     applicationNo: number=0;
     resumeId: string='';
     assignedToUsername = "";
     taskOwnerUsername = "";
     taskStatus ='';
     orderId = 0;
     
     sort = "taskDate";
     pageNumber = 1;
     pageSize = 15;
     search: string='';
}