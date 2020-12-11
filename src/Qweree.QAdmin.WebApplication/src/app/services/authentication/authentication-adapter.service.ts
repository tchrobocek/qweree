import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {TokenInfo} from '../../model/authentication/TokenInfo';
import {TokenInfoDto} from './TokenInfoDto';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationAdapterService {

  constructor(private httpClient: HttpClient) {
  }

  login(username: string, password: string): Promise<TokenInfo> {
    const body = new URLSearchParams();
    body.set('grant_type', 'password');
    body.set('username', username);
    body.set('password', password);

    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
    };

    const response = this.httpClient.post<TokenInfoDto>('http://localhost:8080/api/oauth2/auth', body.toString(), options);
    return response.toPromise()
      .then(t => new TokenInfo(t.access_token, t.refresh_token, t.expires_in, new Date().toDateString()));
  }
}
