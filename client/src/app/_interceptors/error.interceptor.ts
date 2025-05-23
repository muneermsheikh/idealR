import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        const chunkFailedMessage = /Loading chunk [\d]+ failed/;

        if (chunkFailedMessage.test(error.message)) {
          window.location.reload();
        }
        if(error) {
          
          /*const chunkFailedMessage = /Loading chunk [\d]+ failed/;
          if (chunkFailedMessage.test(error.message)) {
            if (confirm("New version available. Load New Version?")) {
                window.location.reload();
            }
          } else {
           */
            switch (error.status) {
              case 400:
                if(error.error.errors) {
                  const modalStateErrors=[];
                  for (const key in error.error.errors) {
                    if(error.error.errors[key]) {
                      modalStateErrors.push(error.error.errors[key])
                    }
                  }
                  throw modalStateErrors.flat();
                } else {
                  this.toastr.error(error.error, error.status.toString(), {closeButton: true, timeOut:15000});
                }
                break;
              case 401:
                this.toastr.error('Unauthorized', error.status.toString());
                break;
              case 402:
                  this.toastr.error('Record not found', error?.statusText.toString());
                  break;
              case 403:
                this.toastr.error('Access Not Authorized', error.status.toString());
                break;
              case 404:
                this.router.navigateByUrl('/not-found');
                break;
              case 500:
                const navigationExtras: NavigationExtras = {state: {error: error.error}};
                this.router.navigateByUrl('/server-error', navigationExtras);
                break;
              default:
                this.toastr.error('Error ' + error.status + ' - something unexpected went wrong');
                console.log(error.error);
                break;
            }
          }
        //}

        throw error;
      })
    )
  }
}
