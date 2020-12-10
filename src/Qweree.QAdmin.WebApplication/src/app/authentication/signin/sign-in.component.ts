import {Component} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {TokenInfo} from '../../model/authentication/TokenInfo';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-sign-in-page',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent {

  public username: string;
  public password: string;

  constructor(private httpClient: HttpClient, private snackBar: MatSnackBar) {
  }

  login(): void {
    const body = new URLSearchParams();
    body.set('grant_type', 'password');
    body.set('username', this.username);
    body.set('password', this.password);

    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
    };

    this.httpClient.post<TokenInfo>('http://localhost:8080/api/oauth2/auth', body.toString(), options)
      .subscribe(response => {
        if (!response) {
          return;
        }
        console.log(response);
      }, () => {
        this.snackBar.open(`Bad credentials`, ``, {duration: 3000});
      });
  }
}
