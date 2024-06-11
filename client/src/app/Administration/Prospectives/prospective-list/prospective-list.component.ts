import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-prospective-list',
  templateUrl: './prospective-list.component.html',
  styleUrls: ['./prospective-list.component.css']
})
export class ProspectiveListComponent {

  form: FormGroup = new FormGroup({})
}
