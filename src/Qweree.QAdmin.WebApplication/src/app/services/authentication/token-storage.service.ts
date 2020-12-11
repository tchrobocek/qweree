import {Injectable} from '@angular/core';
import {TokenInfo} from '../../model/authentication/TokenInfo';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {

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
