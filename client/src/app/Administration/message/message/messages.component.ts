import { Component, OnInit } from '@angular/core';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { Message } from 'src/app/_models/message';
import { Pagination } from 'src/app/_models/pagination';
import { messageParams } from 'src/app/_models/params/Admin/messageParams';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {

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

  constructor(private service: MessageService) { }

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

  removeMessage(msgtext: Message) {

  }

  sendMessage() {

  }

}
