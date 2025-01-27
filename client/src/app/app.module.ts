import { NO_ERRORS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { HomeComponent } from './home/home.component';
import { SharedModule } from './_modules/shared.module';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { IdsModalComponent } from './modals/ids-modal/ids-modal.component';
import { HelpModalComponent } from './modals/help-modal/help-modal.component';
//material includes
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ModalModule } from 'ngx-bootstrap/modal';
import { SelectAssociatesModalComponent } from './select-associates-modal/select-associates-modal.component';
import { RolesModalComponent } from './modals/roles-modal/roles-modal.component';
import { InputModalComponent } from './modals/input-modal/input-modal.component';
import { DisplayTextModalComponent } from './modals/display-text-modal/display-text-modal.component';
import { DateInputRangeModalComponent } from './modals/date-input-range-modal/date-input-range-modal.component';
import { CandidatesAvailableModalComponent } from './modals/candidates-available-modal/candidates-available-modal.component';
import { CvAssessModalComponent } from './hr/cv-assess-modal/cv-assess-modal.component';


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    TestErrorComponent,
    NotFoundComponent,
    ServerErrorComponent,
    UserManagementComponent,
    PhotoManagementComponent,
    ConfirmDialogComponent,
    IdsModalComponent,
    HelpModalComponent,
    //CandidateAssessmentComponent,
    CvAssessModalComponent,
    SelectAssociatesModalComponent,
    
    RolesModalComponent,
    InputModalComponent,
    DisplayTextModalComponent,
    DateInputRangeModalComponent,
    CandidatesAvailableModalComponent,
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
    MatSidenavModule,
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
