import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
//import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';
import { MemberMessageComponent } from '../member-message/member-message.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-details',
  standalone: true,
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css'],
  imports: [CommonModule, TabsModule, TimeagoModule, MemberMessageComponent
    //, GalleryModule
  ]
})

export class MemberDetailsComponent implements OnInit{

  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  
  member: Member = {} as Member;
  
  //images: GalleryItem[]=[];
  activeTab?: TabDirective;
  messages: Message[] = [];


  constructor(private toastr: ToastrService, private route: ActivatedRoute, private messageService: MessageService) {}

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab']);
      }
    })

    //this.getImages();
  }

  selectTab(heading: string) {
   
    if(this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active=true;
    }

    this.toastr.info(heading + ' clicked');
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages') this.loadMessages();
  }

  loadMessages() {
    if(this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }
    this.toastr.info('messages loaded');
  }


  getImages() {
    if(!this.member) return;
    for(const photo of this.member.photos)
      {
       // this.images.push(new ImageItem({src: photo.url, thumb: photo.url }))
      }
  }
}
