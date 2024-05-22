import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from 'src/environments/environment.development';


@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  uploadWithProgress(formData: FormData): Observable<any> {
      return this.http.post(this.baseUrl + 'fileupload', formData, { observe: 'events',  reportProgress: true })
          .pipe(
              catchError(err => this.handleError(err))
          );
  }
  
  private handleError(error: any) {
      return throwError(error);
  }
}
