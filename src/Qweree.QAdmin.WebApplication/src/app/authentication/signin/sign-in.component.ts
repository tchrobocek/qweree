import {Component, OnInit} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {TokenInfo} from '../../model/authentication/TokenInfo';
import {MatSnackBar} from '@angular/material/snack-bar';
import {TokenStorageService} from '../token-storage.service';
import {Router} from '@angular/router';
import {TokenInfoDto} from './TokenInfoDto';

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
    private router: Router
  ) {
  }

  ngOnInit(): void {
    if (this.tokenStorage.isAuthenticated()) {
      this.router.navigate(['/']);
    }
  }

  login(): void {
    const body = new URLSearchParams();
    body.set('grant_type', 'password');
    body.set('username', this.username);
    body.set('password', this.password);

    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
    };

    this.httpClient.post<TokenInfoDto>('http://localhost:8080/api/oauth2/auth', body.toString(), options)
      .subscribe(response => {
        if (!response) {
          return;
        }

        console.log(response);
        const tokenInfo = new TokenInfo(response.access_token, response.refresh_token, response.expires_in, new Date());
        this.tokenStorage.setTokenInfo(tokenInfo);
        this.router.navigate(['/']);
      }, error => {
        if (error.status >= 400 && error.status < 500) {
          this.snackBar.open(`Bad credentials`, ``, {duration: 2000});
        } else {
          this.snackBar.open(`Server unavailable.`, ``, {duration: 2000});
        }
      });
  }
}
