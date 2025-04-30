import { Component, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent {

  @Input() copyright: string = '';
  @Input() licensedTo: string = '';
  @Input() email: string = '';

  constructor(public bsModalRef: BsModalRef) {}

  
}
