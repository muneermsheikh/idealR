import { IIntervwItem } from "./intervwItem";

export interface IIntervw{
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
     interviewVenues: string;
     interviewItems: IIntervwItem[];
}



