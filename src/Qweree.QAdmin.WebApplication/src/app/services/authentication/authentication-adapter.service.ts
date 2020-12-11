import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {TokenInfo} from '../../model/authentication/TokenInfo';
import {TokenInfoDto} from './TokenInfoDto';
import {catchError, map} from 'rxjs/operators';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationAdapterService {

  constructor(private httpClient: HttpClient) {
  }

  login(username: string, password: string): Observable<TokenInfo> {
    const body = new URLSearchParams();
    body.set('grant_type', 'password');
    body.set('username', username);
    body.set('password', password);

    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
    };

    return this.httpClient.post<TokenInfoDto>('http://localhost:8080/api/oauth2/auth', body.toString(), options)
      .pipe(map(t => {
        return new TokenInfo(t.access_token, t.refresh_token, t.expires_in, new Date().toDateString());
      }, catchError(e => {
        throw e;
      })));
  }
}
