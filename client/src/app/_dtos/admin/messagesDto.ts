import { IMessage } from "src/app/_models/admin/message";


export interface IMessagesDto
{
     emailMessage: IMessage;
     errorString: string;
     cvRefIds: number[];
}