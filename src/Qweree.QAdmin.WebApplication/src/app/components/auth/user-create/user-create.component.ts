import { Component, OnInit } from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';
import {IdentityAdapterService} from '../../../services/authentication/identity-adapter.service';

@Component({
  selector: 'app-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user-create.component.scss']
})
export class UserCreateComponent implements OnInit {

  public username = '';
  public fullName = '';
  public password = '';
  public password2 = '';
  public email = '';
  public roles = {
    AUTH_MANAGE: false,
    AUTH_USERS_READ: false,
    AUTH_USERS_CREATE: false,
    CDN_MANAGE: false,
    CDN_EXPLORE: false,
    CDN_STORAGE_STORE: false,
    AUTH_USERS_READ_PERSONAL_DETAIL: false
  };

  constructor(
    private snackbar: MatSnackBar,
    private identityAdapter: IdentityAdapterService
  ) { }

  ngOnInit(): void {
  }

  create(): void {
    if (this.password !== this.password2) {
      this.snackbar.open('Passwords must match.');
      return;
    }

    const userInput = {
      username: this.username,
      contactEmail: this.email,
      fullName: this.fullName,
      password: this.password,
      roles: []
    };

    Object.keys(this.roles).forEach(k => {
      if (this.roles[k]) {
        userInput.roles.push(k);
      }
    });

    this.identityAdapter.createUser(userInput).subscribe(r => {
      this.snackbar.open('User "' + r.username + '" created.', 'X', { duration: 1500 });
    }, err => {
      err.error.errors.map(e => e.message).forEach(m => {
        this.snackbar.open(m, 'X', { duration: 1500 });
      });
    });
  }
}
