import { User } from "../user";


export class UserParams {
    gender: string;
    //minAge = 18;
    //maxAge = 75;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'lastActive';

    constructor(user: User) {
        this.gender  = "male"   //user.gender  //=== 'female' ? 'male' : 'female'
    }
    
}