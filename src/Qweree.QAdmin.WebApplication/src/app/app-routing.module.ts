import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {SignInComponent} from './authentication/signin/sign-in.component';
import {DashboardComponent} from './dashboard/dashboard/dashboard.component';
import {AuthGuardService} from './authentication/auth-guard.service';


const routes: Routes = [
  {
    path: 'signin',
    component: SignInComponent
  },
  {
    path: ``,
    component: DashboardComponent,
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
