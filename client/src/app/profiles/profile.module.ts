import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from './profile-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { ChecklistModalComponent } from './checklist-modal/checklist-modal.component';
import { CandidateEditComponent } from './candidate-edit/candidate-edit.component';
import { CvsAvailableToRefComponent } from './cvs-available-to-ref/cvs-available-to-ref.component';
import { ProfileListComponent } from './profile-list/profile-list.component';
import { ProfileItemComponent } from './profile-item/profile-item.component';
import { ProfileMenuComponent } from './profile-menu/profile-menu.component';
import { AvailableItemComponent } from './available-item/available-item.component';
import { FileuploadComponent } from './fileupload/fileupload.component';
import { CvsreferredComponent } from './cvsreferred/cvsreferred.component';
import { CvreferredLineComponent } from './cvreferred-line/cvreferred-line.component';


@NgModule({
  declarations: [
    ChecklistModalComponent,
    CandidateEditComponent,
    CvsAvailableToRefComponent,
    ProfileListComponent,
    ProfileItemComponent,
    ProfileMenuComponent,
    AvailableItemComponent,
    FileuploadComponent,
    CvsreferredComponent,
    CvreferredLineComponent,
   
  ],
  imports: [
    CommonModule,
    ProfileRoutingModule,
    SharedModule,
    //AccordionModule
  ]
})
export class ProfileModule { }
