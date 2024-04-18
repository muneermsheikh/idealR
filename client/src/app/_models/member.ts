import { Photo } from "./photo";

export interface Member
{
    "id": number,
    "gender": string,
    "userName": string,
    "knownAs": string,
    "age": number,
    "created": Date,
    "lastActive": Date,
    "city": string,
    "country": string,
    "interests": string,
    "lookingFor": string,
    "introduction": string,
    "photoUrl": string,
    "photos": Photo[]
}