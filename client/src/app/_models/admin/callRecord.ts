import { ICallRecordItem } from "./callRecordItem";


export interface ICallRecord 
{
     checked: false;
     id: number;
     categoryRef: string;
     personType: string;
     personId: string;
     personName: string;
     subject: string;
     phoneNo: string;
     Email: string;
     status: string;
     statusDate?: Date;
     createdOn: Date;
     username: string;
     concludedOn?: Date;
     callRecordItems: ICallRecordItem[];
}