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
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CvAssessComponent } from './cv-assess/cv-assess.component';


@NgModule({
  declarations: [
    ChecklistModalComponent,
    CandidateEditComponent,
    CvAssessComponent,
    CvsAvailableToRefComponent,
    ProfileListComponent,
    ProfileItemComponent,
    ProfileMenuComponent,
    AvailableItemComponent,
    FileuploadComponent,
    CvsreferredComponent,
    CvreferredLineComponent,
    CvAssessComponent
  ],
  imports: [
    CommonModule,
    ProfileRoutingModule,
    SharedModule,
    MatIconModule
    //, MatButtonModule
  ]
})
export class ProfileModule { }
