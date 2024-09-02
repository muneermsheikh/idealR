import { IHRSkill } from "./hrSkill";
import { IEmployeeOtherSkill } from "./employeeOtherSkill";
import { IEmployeeAttachment } from "./employeeAttachment";

export interface IEmployee {
     id: number;
     gender: string;
     firstName: string;
     secondName: string;
     familyName: string;
     knownAs: string;
     displayName: string;
     address: string;
     address2: string;
     city: string;
     //pin: string;
     //state: string;
     
     email: string;
     phoneNo: string;
     phone2: string;
     dateOfBirth: Date;
     dateOfJoining: Date;     

     placeOfBirth: string;
     department: string;
     aadharNo: string;
     introduction: string;
     position: string;
     appUserId: number;
     qualification: string;
    
     userName: string;
     //remarks: string;
     status: string;

     hrSkills: IHRSkill[];
     employeeOtherSkills: IEmployeeOtherSkill[];
     employeeAttachments: IEmployeeAttachment[];
}    

export class Employee implements IEmployee {
     id= 0;
     gender= 'M';
     firstName= '';
     secondName= '';
     familyName= '';
     knownAs= '';
     displayName= '';
     address='';
     address2= '';
     city= '';
     //pin= '';
     //state= '';
     
     email= '';
     phoneNo = '';
     phone2 = '';
     dateOfBirth = new Date;
     dateOfJoining= new Date;     

     placeOfBirth= '';
     department= '';
     aadharNo= '';
     introduction= '';
     position= '';
     appUserId= 0;
     qualification = '';
     userName= '';
     //remarks='';
     status='';

     hrSkills: IHRSkill[]=[];
     employeeOtherSkills: IEmployeeOtherSkill[]=[];
     employeeAttachments: IEmployeeAttachment[]=[];
}    
