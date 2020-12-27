import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {SignInComponent} from './components/auth/sign-in/sign-in.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {MatDividerModule} from '@angular/material/divider';
import {FormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {DashboardComponent} from './components/dashboard/dashboard.component';
import {DashboardComponent as CdnDashboardComponent} from './components/cdn/dashboard/dashboard.component';
import {DashboardComponent as AuthDashboardComponent} from './components/auth/dashboard/dashboard.component';
import {AdminShellComponent} from './components/layout/admin-shell/admin-shell.component';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatListModule} from '@angular/material/list';
import { NotFoundComponent } from './components/layout/not-found/not-found.component';
import { ServicesOverviewComponent } from './components/dashboard/services-overview/services-overview.component';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatIconModule} from '@angular/material/icon';
import { StorageExplorerComponent } from './components/cdn/storage-explorer/storage-explorer.component';
import { PathExplorerComponent } from './components/cdn/storage-explorer/path-explorer/path-explorer.component';
import { MediaTypePipe } from './services/pipes/media-type.pipe';
import { FilenamePipe } from './services/pipes/filename.pipe';
import { LinkComponent } from './components/link/link.component';
import {BinarySizePipe} from './services/pipes/binary-size.pipe';
import {MAT_TOOLTIP_SCROLL_STRATEGY_FACTORY_PROVIDER, MatTooltipModule} from '@angular/material/tooltip';
import { StorageImportComponent } from './components/cdn/storage-import/storage-import.component';
import { FileIconPipe } from './services/pipes/file-icon.pipe';
import {NgxFileDropModule} from 'ngx-file-drop';
import {AuthorizationInterceptor} from './services/http/authorization-interceptor.service';
import { UsersExplorerComponent } from './components/auth/users-explorer/users-explorer.component';
import {MatTableModule} from '@angular/material/table';
import {CdkColumnDef} from '@angular/cdk/table';
import { UsersListComponent } from './components/auth/users-list/users-list.component';
import { UserCreateComponent } from './components/auth/user-create/user-create.component';
import {MatTabsModule} from '@angular/material/tabs';
import { UserInfoPageComponent } from './components/auth/user-info-page/user-info-page.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    DashboardComponent,
    AdminShellComponent,
    NotFoundComponent,
    ServicesOverviewComponent,
    CdnDashboardComponent,
    AuthDashboardComponent,
    StorageExplorerComponent,
    PathExplorerComponent,
    MediaTypePipe,
    FilenamePipe,
    BinarySizePipe,
    LinkComponent,
    StorageImportComponent,
    FileIconPipe,
    UsersExplorerComponent,
    UsersListComponent,
    UserCreateComponent,
    UserInfoPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDividerModule,
    MatSnackBarModule,
    FormsModule,
    HttpClientModule,
    MatSidenavModule,
    MatCheckboxModule,
    MatListModule,
    MatGridListModule,
    MatIconModule,
    MatTooltipModule,
    NgxFileDropModule,
    MatTableModule,
    MatTabsModule,
  ],
  providers: [
    MAT_TOOLTIP_SCROLL_STRATEGY_FACTORY_PROVIDER,
    {
      provide : HTTP_INTERCEPTORS,
      useClass: AuthorizationInterceptor,
      multi   : true,
    },
    CdkColumnDef
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
