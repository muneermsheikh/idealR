import { IDepItem } from "./depItem";

export interface IDep
{
	id: number;
	cvRefId: number;
	orderItemId: number;
	customerId: number;
	customerName: string;
	cityOfWorking: string;
	selectedOn: Date;
	currentStatus: string;
	depItems: IDepItem[];
	ecnr: boolean;

}


export class Dep implements IDep
{
	id: number=0;
	cvRefId: number=0;
	orderItemId: number=0;
	customerId: number=0;
	customerName: string ='';
	cityOfWorking: string = '';
	selectedOn: Date = new Date();
	currentStatus: string = '';
	depItems: IDepItem[] = [];
	ecnr = false;
}