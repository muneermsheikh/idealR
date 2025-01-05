export interface ICOA{
	id: number;
	section: string;
	divn: string;
	accountType: string;
	accountName: string;
	accountClass: string;
	opBalance: number;
}

export class coa implements ICOA{
	id=0;
	section = '';
	divn='';
	accountType='';
	accountName='';
	accountClass='';
	opBalance=0;
}