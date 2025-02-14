import { booleanAttribute, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { SetAudioText } from 'src/app/_dtos/admin/setAudioText';
import { IAudioMessageDto } from 'src/app/_dtos/hr/audioMessageDto';
import { Pagination } from 'src/app/_models/pagination';
import { audioMessageParams } from 'src/app/_models/params/audioMessageParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { AudioService } from 'src/app/_services/audio.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { InputModalComponent } from 'src/app/modals/input-modal/input-modal.component';

@Component({
  selector: 'app-autodial',
  templateUrl: './autodial.component.html',
  styleUrls: ['./autodial.component.css']
})
export class AutodialComponent implements OnInit {

    user?: User;
    returnUrl = '';
  
    audioMessages: IAudioMessageDto[]=[];
    
    pagination: Pagination | undefined;
        
    totalCount=0;
   
    pParams = new audioMessageParams();
    viewMessageText=false;
    
    bsModalRef: BsModalRef | undefined;
    AllSelected=false;

    constructor(private service: AudioService, private toastr: ToastrService, private confirm: ConfirmService,
        private activatedRoute: ActivatedRoute, private accountService: AccountService, private modalService: BsModalService,){
        
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
      }

    ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => {
        this.audioMessages = data['audioMessages'].result,
        this.pagination = data['pagination'],
        this.totalCount = data['audioMessages'].totalCount
      })

      this.loadMessages();
    }

    loadMessages() {
      var params = this.service.getParams();
      this.service.setParams(params!);
      this.service.getAudioMessagesPaged(true)?.subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.audioMessages = response.result;
          this.totalCount = response?.count;
          this.pagination = response.pagination;
  
        } else {
          console.log('response is undefined');
        }
      },
      error: error => console.log(error)
     })
     
    }
    onPageChanged(event: any){
      const params = this.service.getParams();
      if (params!.pageNumber !== event) {
        params!.pageNumber = event.page;
        this.service.setParams(params!);
  
        this.loadMessages();
      }
    }

    editMessageText(id: number, msgText: string) 
    {
        const config = {
            class:'modal-dialog-centered modal-md',
            initialState: {
              title: 'Edit message text',
              inputValue: msgText
            }
        };
          
        this.bsModalRef = this.modalService.show(InputModalComponent, config);
      
        this.bsModalRef.content.outputEvent.subscribe((txt: any) => {
            if(txt !== '') {
              var index = this.audioMessages.findIndex(x => x.id==id);
              if(index !== -1) this.audioMessages[index].messageText=txt;

              var audioTxt:SetAudioText = {id: id, textMessage: txt};
              this.service.setAudioText(audioTxt).subscribe({
                next: (succeeded: boolean) => this.toastr.success('message text updated', 'Success')
              })
            }});
    
    }

    selectedChanged() {
      this.audioMessages.forEach(x => {
        x.checked = this.AllSelected;
      })
    }

    update() {
        var selectedRecords = this.audioMessages.filter(x => x.checked);

        console.log(selectedRecords);
    }
}
