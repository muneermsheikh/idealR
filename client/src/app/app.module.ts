import { NO_ERRORS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';

import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages/messages.component';
import { SharedModule } from './_modules/shared.module';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { RegisterComponent } from './register/register.component';
import { MemberLikedListComponent } from './members/member-liked-list/member-liked-list.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { RolesModalComponent } from './modals/roles-modal/roles-modal.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { IdsModalComponent } from './modals/ids-modal/ids-modal.component';
import { HelpModalComponent } from './modals/help-modal/help-modal.component';
import { CvAssessComponent } from './hr/cv-assess/cv-assess.component';
import { CandidateAssessmentComponent } from './hr/candidate-assessment/candidate-assessment.component';
import { CandidateAssessComponent } from './hr/candidate-assess/candidate-assess.component';
import { CvAssessModalComponent } from './hr/cv-assess-modal/cv-assess-modal.component';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    MessagesComponent,
    TestErrorComponent,
    NotFoundComponent,
    ServerErrorComponent,
    MemberListComponent,
    //MemberDetailsComponent,
    MemberCardComponent,
    MemberEditComponent,
    RegisterComponent,
    MemberLikedListComponent,
    HasRoleDirective,
    UserManagementComponent,
    PhotoManagementComponent,
    RolesModalComponent,
    ConfirmDialogComponent,
    IdsModalComponent,
   
    HelpModalComponent,
    CvAssessComponent,
    CandidateAssessmentComponent,
   
    CandidateAssessComponent,
    CvAssessModalComponent,
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    //FormsModule,
    //ReactiveFormsModule,
    SharedModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true}
  ],
  bootstrap: [AppComponent],
  schemas: [NO_ERRORS_SCHEMA]
})
export class AppModule { }
