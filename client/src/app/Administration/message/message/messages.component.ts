import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { Pagination } from 'src/app/_models/pagination';
import { messageParams } from 'src/app/_models/params/Admin/messageParams';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {

  @ViewChild('searchInUsernames', {static: false}) searchTermInUsernames: ElementRef | undefined;
  @ViewChild('searchInContents', {static: false}) searchTermInContents: ElementRef | undefined;
  
  messages?: Message[] = [];
  msgParams = new messageParams();
  pagination?: Pagination;
  container = "Inbox";
  loading = false;
  messageSentOn: Date = new Date;
  iMessageId: number=0;

  msgToDisplay: string='';

  lastContainer = "";

  editorConfig: AngularEditorConfig = {
    editable: true,
      spellcheck: true,
      height: 'auto',
      minHeight: '0',
      maxHeight: 'auto',
      width: 'auto',
      minWidth: '0',
      translate: 'yes',
      enableToolbar: true,
      showToolbar: true,
      placeholder: 'Enter text here...',
      defaultParagraphSeparator: '',
      defaultFontName: '',
      defaultFontSize: '',
      fonts: [
        {class: 'arial', name: 'Arial'},
        {class: 'times-new-roman', name: 'Times New Roman'},
        {class: 'calibri', name: 'Calibri'},
        {class: 'comic-sans-ms', name: 'Comic Sans MS'}
      ],
      customClasses: [
      {
        name: 'quote',
        class: 'quote',
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: 'titleText',
        class: 'titleText',
        tag: 'h1',
      }
    ]
    //, uploadUrl: 'v1/image',
    //upload: (file: File) => { ... }
    , uploadWithCredentials: false,
    sanitize: true,
    toolbarPosition: 'top',
    toolbarHiddenButtons: [
      ['bold', 'italic'],
      ['fontSize']
    ]
};

  constructor(private service: MessageService, private toastr: ToastrService,
      private confirm: ConfirmService) { }

  ngOnInit(): void {
      //this.loadMessages()
  }

  loadMessages(useCache: boolean=true) {
    if(this.container == this.lastContainer) return;

    this.loading = true;
    this.service.setParams(this.msgParams);

    this.service.getMessages(useCache).subscribe({
      next: response => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
      }
    })

    this.lastContainer = this.container;
  }

  deleteMessage(id: number) {
    this.service.deleteMessage(id).subscribe({
      next: _ => this.messages?.splice(this.messages.findIndex(m => m.id === id), 1)
    })
  }

  pageChanged(event: any) {
    if (this.msgParams.pageNumber !== event.page) {
      this.msgParams.pageNumber = event.page;
      this.loadMessages();
    }
  }

  msgClicked(messageId: number, msgText: string, messageSentOn: Date) {
    this.msgToDisplay=msgText;
    this.messageSentOn = new Date(messageSentOn);
    this.iMessageId = messageId;
  }

  removeMessage(msgId: number) {
    var confirmMsg = 'confirm delete this Message. ' +
      'WARNING: this cannot be undone';

    const observableInner = this.service.deleteMessage(msgId);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe({
      next: (response) => {
        console.log('deleted response:', response);
        if(response === 'Unauthorized') {
          this.toastr.warning('The message can be deleted by the sender or the recipient', 'Unauthorized')
        } else if (response === 'Deleted' || response === 'Marked as Deleted') {
          this.toastr.success('Message Deleted', 'Deletion successful')
          var index = this.messages?.findIndex(x => x.id == msgId);
          if(index !==-1) this.messages!.splice(index!,1);
        } else {
          this.toastr.error(response, 'Failed to delete the message')
        }
      },
      error: (err: any) => this.toastr.error(err.error.text, 'Error encountered')
    })
      
  }

  sendMessage() {

  }

  onSearchInUsernames() {
    const params = new messageParams(); // this.service.getParams();
    params.search = this.searchTermInUsernames?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.loadMessages();
  }

  onSearchInContents() {
    const params = new messageParams(); // this.service.getParams();
    params.searchInContents = this.searchTermInContents?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.loadMessages();
  }

}