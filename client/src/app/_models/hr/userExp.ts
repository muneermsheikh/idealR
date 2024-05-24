export interface IUserExp {
     id: number;
     candidateId: number;
     srNo: number;
     position: string;
     employer: string;
     currentJob?: boolean;
     salaryCurrency: string;
     monthlySalaryDrawn?: number;
     workedFrom: Date;
     workedUpto?: Date;
}