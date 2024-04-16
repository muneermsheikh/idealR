import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { CandidateListComponent } from './candidates/candidate-list/candidate-list.component';
import { CandidateDetailsComponent } from './candidates/candidate-details/candidate-details.component';
import { MessagesComponent } from './messages/messages/messages.component';
import { authGuard } from './_guards/auth.guard';

const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard,],
    children: [
      {path: 'candidates', component: CandidateListComponent, canActivate: [authGuard]},
      {path: 'candidates/:id', component: CandidateDetailsComponent},
      {path: 'messages', component:MessagesComponent},
      ]
  },
  {path: '**', component: HomeComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
