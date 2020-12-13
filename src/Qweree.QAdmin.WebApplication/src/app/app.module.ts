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
import {HttpClientModule} from '@angular/common/http';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {DashboardComponent} from './components/dashboard/dashboard.component';
import {DashboardComponent as CdnDashboardComponent} from './components/cdn/dashboard/dashboard.component';
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

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    DashboardComponent,
    AdminShellComponent,
    NotFoundComponent,
    ServicesOverviewComponent,
    CdnDashboardComponent,
    StorageExplorerComponent,
    PathExplorerComponent,
    MediaTypePipe,
    FilenamePipe,
    BinarySizePipe,
    LinkComponent
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
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
