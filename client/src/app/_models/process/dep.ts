import { IDepItem } from "./depItem";

export interface IDep
{
	id: number;
	cVRefId: number;
	orderItemId: number;
	customerId: number;
	selectedOn: Date;
	currentStatus: string;
	depItems: IDepItem[];

}


export class Dep implements IDep
{
	id: number=0;
	cVRefId: number=0;
	orderItemId: number=0;
	customerId: number=0;
	selectedOn: Date = new Date();
	currentStatus: string = '';
	depItems: IDepItem[] = [];

}