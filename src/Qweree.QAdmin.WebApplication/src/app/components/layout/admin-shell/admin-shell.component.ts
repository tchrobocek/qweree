import {Component, OnInit} from '@angular/core';
import {TokenStorageService} from '../../../services/authentication/token-storage.service';
import {Router} from '@angular/router';
import jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-admin-shell',
  templateUrl: './admin-shell.component.html',
  styleUrls: ['./admin-shell.component.scss']
})
export class AdminShellComponent implements OnInit {

  public userId = '';
  public username = '';
  public roles: string[] = [];
  public profileMenuVisible = false;

  constructor(
    private tokenStorage: TokenStorageService,
    public router: Router
  ) {

  }

  ngOnInit(): void {
    const jwt = this.tokenStorage.getTokenInfo().accessToken ?? '';
    const token = jwt_decode(jwt) as any;

    this.userId = token.userId;
    this.username = token.username;
    this.roles = token.role;
  }

  signOut(): void {
    this.tokenStorage.removeTokenInfo();
    this.router.navigate([`sign-in`]);
  }
}
