export interface IProfession {
     id: number;
     professionName: string;
     professionGroup: string;
}

export class Profession implements IProfession {
     id: number = 0;
     professionName: string = '';
     professionGroup: string = '';
}


