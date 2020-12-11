import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';
import {TokenStorageService} from '../../../services/authentication/token-storage.service';
import {Router} from '@angular/router';
import {AuthenticationAdapterService} from '../../../services/authentication/authentication-adapter.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent implements OnInit {

  public username: string;
  public password: string;

  constructor(
    private httpClient: HttpClient,
    private snackBar: MatSnackBar,
    private tokenStorage: TokenStorageService,
    private router: Router,
    private authenticationAdapter: AuthenticationAdapterService
  ) {
  }

  ngOnInit(): void {
    if (this.tokenStorage.isAuthenticated()) {
      this.router.navigate(['/']);
    }
  }

  login(): void {
    if (!this.username || !this.password) {
      return;
    }

    this.authenticationAdapter.login(this.username, this.password)
      .subscribe(t => {
        this.tokenStorage.setTokenInfo(t);
        this.router.navigate(['/']);
      }, error => {
        this.password = '';
        if (error.status >= 400 && error.status < 500) {
          this.snackBar.open(`Bad credentials`, ``, {duration: 2000});
        } else {
          this.snackBar.open(`Server unavailable.`, ``, {duration: 2000});
        }
      });
  }
}
