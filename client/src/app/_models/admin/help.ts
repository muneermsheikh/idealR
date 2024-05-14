import { IHelpItem } from "./helpItem";

export interface IHelp
{
	id: number;
	topic: string;
	helpItems: IHelpItem[];
}

export class Help implements IHelp
{
	id = 0;
	topic = '';
	helpItems: IHelpItem[] = [];
}