import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { IHelp } from '../_models/admin/help';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  baseUrl = environment.apiUrl;
  bsModelRef?: BsModalRef<ConfirmDialogComponent>;
  
  constructor(private modalService: BsModalService, private http: HttpClient) { }

  confirm(
    title = 'Confirmation', 
    message = 'Are you sure you want to do this?', 
    btnOkText = 'Ok', 
    btnCancelText = 'Cancel'): Observable<boolean> {
      const config = {
        initialState: {
          title, 
          message,
          btnOkText,
          btnCancelText
        }
      }
      this.bsModelRef = this.modalService.show(ConfirmDialogComponent, config);
      return this.bsModelRef.onHidden!.pipe(
        map(() => {
          return this.bsModelRef!.content!.result
        })
      )
  }

  
  getHelp(topic: string)
  {
    return this.http.get<IHelp>(this.baseUrl + 'Help/help/' + topic);
  }
}
