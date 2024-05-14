
export interface IInterviewBrief{
     id: number;
     checked: boolean;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     companyName: string;
     interviewVenue: string;
     interviewDateFrom: Date;
     interviewDateUpto: Date;
     interviewStatus: string;
     concludingRemarks: string;
     //interviewItems: IInterviewItem[];
}



