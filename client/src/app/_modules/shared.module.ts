import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ToastrModule } from 'ngx-toastr';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons'
import { TimeagoModule } from 'ngx-timeago';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { HasRoleDirective } from '../_directives/has-role.directive';
import { RegisterCallRecordsComponent } from '../callRecords/register-call-records/register-call-records.component';



@NgModule({
  declarations: [
    TextInputComponent,
    DatePickerComponent,
    HasRoleDirective,
    RegisterCallRecordsComponent
  ],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    NgxSpinnerModule.forRoot({
      type: 'line-scale-party'
    }),
    BsDatepickerModule.forRoot(),
    ToastrModule.forRoot(),
    /*ToastrModule.forRoot({
      "closeButton":true,
      "positionClass": 'toastr-top-right',
      "preventDuplicates": true,
      
    }),*/
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    TimeagoModule.forRoot(),
    ModalModule.forRoot(),
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
  ],
  
  exports: [
    BsDropdownModule,
    TabsModule,
    NgxSpinnerModule,
    BsDatepickerModule,
    PaginationModule,
    ButtonsModule,
    TimeagoModule,
    ToastrModule,
    ModalModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    TextInputComponent,
    DatePickerComponent,
    HasRoleDirective,
    RegisterCallRecordsComponent
  ]
})
export class SharedModule { }
