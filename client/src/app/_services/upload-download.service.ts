import { HttpClient, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';


@Injectable({
  providedIn: 'root'
})
export class UploadDownloadService {

  apiUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

  /*
  uploadFile(theFiles: FileToUpload[]) : Observable<any> {
    
    const uploadReq = new HttpRequest('POST', `api/FileUpload`, theFiles, {
      reportProgress: true,
    });

    return this.http.post<FileToUpload>(this.apiUrl + 'FileUpload', theFiles,  httpOptions);
  }
  */

  downloadFile(attachmentid: number) {
    return this.http.get(this.apiUrl + 'candidate/downloadfile/' + attachmentid, {responseType: 'blob'});
  }

  
  downloadProspectiveFile(prospectiveid: number) {

    return this.http.get(this.apiUrl + 'FileUpload/downloadprospectivefile/' + prospectiveid);
  }
}
