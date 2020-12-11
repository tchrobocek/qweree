import { Component, OnInit } from '@angular/core';
import {TokenStorageService} from '../../../services/authentication/token-storage.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-admin-shell',
  templateUrl: './admin-shell.component.html',
  styleUrls: ['./admin-shell.component.scss']
})
export class AdminShellComponent implements OnInit {

  constructor(
    private tokenStorage: TokenStorageService,
    private router: Router
  ) {

  }

  ngOnInit(): void {
  }

  signOut(): void {
    this.tokenStorage.removeTokenInfo();
    this.router.navigate([`sign-in`]);
  }
}
