import { NO_ERRORS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';

import { HomeComponent } from './home/home.component';
import { SharedModule } from './_modules/shared.module';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
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
//material includes
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SelectAssociatesModalComponent } from './modals/select-associates-modal/select-associates-modal.component';
import { InputModalComponent } from './modals/input-modal/input-modal.component';
import { DateInputRangeModalComponent } from './modals/date-input-range-modal/date-input-range-modal.component';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    TestErrorComponent,
    NotFoundComponent,
    ServerErrorComponent,
  
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
    SelectAssociatesModalComponent,
    InputModalComponent,
    DateInputRangeModalComponent,
  
        
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,

    //material
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatExpansionModule,
    MatTooltipModule,
    
    //FormsModule,
    //ReactiveFormsModule,
    SharedModule,
  
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
