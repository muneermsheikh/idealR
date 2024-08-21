
export interface IInterviewBrief{
     id: number;
     checked: boolean;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     customerName: string;
     interviewVenues: string;
     interviewDateFrom: Date;
     interviewDateUpto: Date;
     interviewStatus: string;
     //interviewItems: IInterviewItem[];
}



