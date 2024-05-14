export class AppUserParams {
     loggedInEmployeeId = 0;
     objectId = 0;
     username = '';
     email = '';
     displayName = '';
     token = '';
     roles : string[]= [];

     sort = "name";
     pageNumber = 1;
     pageSize = 10;
     search: string='';
}