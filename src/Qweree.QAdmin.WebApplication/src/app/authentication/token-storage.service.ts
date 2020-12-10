import {Injectable} from '@angular/core';
import {TokenInfo} from '../model/authentication/TokenInfo';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {

  private token: TokenInfo;

  getTokenInfo(): TokenInfo {
    return this.token;
  }

  setTokenInfo(token: TokenInfo): void {
    this.token = token;
  }

  isAuthenticated(): boolean {
    const token = this.getTokenInfo();
    return token !== undefined;
  }
}
