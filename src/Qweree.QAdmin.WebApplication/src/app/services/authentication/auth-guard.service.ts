import { Injectable } from '@angular/core';
import {TokenStorageService} from './token-storage.service';
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';

// noinspection JSUnusedLocalSymbols
@Injectable({
  providedIn: 'root'
})
export class AuthGuardService {

  constructor(private tokenStorageService: TokenStorageService, private router: Router) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (this.tokenStorageService.isAuthenticated()) {
      return true;
    }

    this.router.navigate(['/sign-in']);
    return false;
  }
}
