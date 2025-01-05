import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { IApiReturnDto } from '../_dtos/admin/apiReturnDto';


@Injectable({
  providedIn: 'root'
})
export class FileService {

  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  public updateWithFiles(formData: FormData) {
    return this.http.put(this.apiUrl + 'candidate/updatecandidate', formData, {
      reportProgress: true,
      observe: 'events',
    });
  }

  public registerNewCandidate(formData: FormData) {
      return this.http.post<IApiReturnDto>(this.apiUrl +  'account/RegisterNewCandidate', formData, {
        reportProgress: true,
        observe: 'events',
    });
  }

  public download(attachmentid: number) {
    return this.http.get(this.apiUrl + 'FileUpload/downloadattachmentfile/' + attachmentid, {
      reportProgress: true,
      //observe: 'events',
      responseType: 'blob',
    });
  }

  downloadCV(candidateId: number) {
    return this.http.get(this.apiUrl + 'FileUpload/downloadcv/' + candidateId, {
      reportProgress: true,
      //observe: 'events',
      responseType: 'blob',
    });
  }

  /*public downloadFile(attachmentid: number) {
    
      const headers = new HttpHeaders().set('Accept', 'application/vnd.ms-excel');

      this.http.get(`this.apiUrl + '/FileUpload/downloadbyattachmentid/' + attachmentid`, { headers, responseType: 'blob' })
        .subscribe(
          (response: any) => {
            const contentDispositionHeader: string = response.headers.get('Content-Disposition');
            const fileName = contentDispositionHeader.split(';')[1].trim().split('=')[1];

            saveAs(response.body, fileName);
          },
          (error: any) => {
            console.error('Error: ', error);
            // show error message to the user
          }
        );
    } */

}

function saveAs(blob: Blob, fileName: string) {
  const link = document.createElement('a');
  link.download = fileName;
  link.href = window.URL.createObjectURL(blob);
  link.click();
  window.URL.revokeObjectURL(link.href);
}
