import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { IIntervw } from 'src/app/_models/hr/intervw';

@Component({
  selector: 'app-interview-edit',
  templateUrl: './interview-edit.component.html',
  styleUrls: ['./interview-edit.component.css']
})
export class InterviewEditComponent implements OnInit{

  interview: IIntervw | undefined;

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute) {}
  
  bsValueDate = new Date();

  ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => {
        this.interview = data['interview'];
      })

  }

}