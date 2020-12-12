import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {SignInComponent} from './components/auth/sign-in/sign-in.component';
import {AuthGuardService} from './services/authentication/auth-guard.service';
import {DashboardComponent} from './components/dashboard/dashboard.component';
import {DashboardComponent as CdnDashboardComponent} from './components/cdn/dashboard/dashboard.component';
import {AdminShellComponent} from './components/layout/admin-shell/admin-shell.component';
import {NotFoundComponent} from './components/layout/not-found/not-found.component';
import {StorageExplorerComponent} from './components/cdn/storage-explorer/storage-explorer.component';


const routes: Routes = [
  {
    path: 'sign-in',
    component: SignInComponent
  },
  {
    path: '',
    component: AdminShellComponent,
    canActivate: [AuthGuardService],
    children: [{
        path: '', component: DashboardComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'cdn', component: CdnDashboardComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'cdn/explorer', component: StorageExplorerComponent,
        canActivate: [AuthGuardService]
      }, {
        path: '**', component: NotFoundComponent
      }]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
