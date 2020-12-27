import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {SignInComponent} from './components/auth/sign-in/sign-in.component';
import {AuthGuardService} from './services/authentication/auth-guard.service';
import {DashboardComponent} from './components/dashboard/dashboard.component';
import {DashboardComponent as CdnDashboardComponent} from './components/cdn/dashboard/dashboard.component';
import {DashboardComponent as AuthDashboardComponent} from './components/auth/dashboard/dashboard.component';
import {AdminShellComponent} from './components/layout/admin-shell/admin-shell.component';
import {NotFoundComponent} from './components/layout/not-found/not-found.component';
import {StorageExplorerComponent} from './components/cdn/storage-explorer/storage-explorer.component';
import {StorageImportComponent} from './components/cdn/storage-import/storage-import.component';
import {UsersExplorerComponent} from './components/auth/users-explorer/users-explorer.component';
import {UserCreateComponent} from './components/auth/user-create/user-create.component';
import {UserInfoPageComponent} from './components/auth/user-info-page/user-info-page.component';
import {ProfileMainPageComponent} from './components/profile/profile-main-page/profile-main-page.component';


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
        path: 'profile', component: ProfileMainPageComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'cdn', component: CdnDashboardComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'cdn/import', component: StorageImportComponent,
        canActivate: [AuthGuardService],
      }, {
        path: 'cdn/explorer',
        children: [{
          path: '**', component: StorageExplorerComponent,
          canActivate: [AuthGuardService]
        }],
      }, {
        path: 'auth', component: AuthDashboardComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'auth/users', component: UsersExplorerComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'auth/users/create', component: UserCreateComponent,
        canActivate: [AuthGuardService]
      }, {
        path: 'auth/users/:id', component: UserInfoPageComponent,
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
