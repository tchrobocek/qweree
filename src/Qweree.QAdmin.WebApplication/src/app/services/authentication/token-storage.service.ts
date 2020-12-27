import {Injectable} from '@angular/core';
import {TokenInfo, UserInfo} from '../../model/authentication/TokenInfo';
import jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {

  getUserInfo(): UserInfo {
    const tokenInfo = this.getTokenInfo();
    return jwt_decode(tokenInfo.accessToken) as UserInfo;
  }

  getTokenInfo(): TokenInfo {
    const json = window.localStorage.getItem(`token_info`);
    if (json === undefined || json === `` || json === null) {
      return undefined;
    }
    return JSON.parse(json);
  }

  setTokenInfo(token: TokenInfo): void {
    window.localStorage.setItem(`token_info`, JSON.stringify(token));
  }

  removeTokenInfo(): void {
    window.localStorage.removeItem(`token_info`);
  }

  isAuthenticated(): boolean {
    const token = this.getTokenInfo();
    if (token === undefined) {
      return false;
    }

    const now = new Date();
    const createdData = new Date(token.createdAt);
    return (createdData.getTime() + token.expiresAt) > now.getTime();
  }
}
