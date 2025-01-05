import { ICallRecordItem } from "src/app/_models/admin/callRecordItem";

export interface ICallRecordItemToAddDto
{
    personId: string;
    personType: string;
    categoryRef: string;
    adviseByMail: boolean;
    adviseBySMS: boolean;
    callRecordItem: ICallRecordItem;
}