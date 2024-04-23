import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  //changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessageComponent implements OnInit {

  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;

  @Input() messages: Message[] = [];
  messageContent = '';
  
  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
    console.log('messages', this.messages)
  }

 
  sendMessage() {

    if (!this.username) return;
   
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: msg => {
        this.messages.push(msg);
        this.messageForm?.reset();
      }
    })
    
  }

}
