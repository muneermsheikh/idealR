import { Photo } from "./photo";

export interface Member
{
    id: number,
    gender: string,
    position: string,
    userName: string,
    knownAs: string,
    age: number,
    dateOfBirth: Date,
    created: Date,
    lastActive: Date,
    city: string,
    country: string,
    email: string,
    phoneNumber: string,
    roles: string[]
    //"interests": string,
    //"lookingFor": string,
    //"introduction": string,
    //"photoUrl": string,
    //"photos": Photo[]
}