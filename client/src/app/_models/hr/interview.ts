import {IInterviewItem} from "./interviewItem";

export interface IInterview{
     id: number;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     customerName: string;
     interviewMode: string;
     interviewDateFrom: Date;
     interviewDateUpto: Date;
     interviewStatus: string;
     interviewItems: IInterviewItem[];
}



