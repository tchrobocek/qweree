import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {IdentityAdapterService} from '../../../services/authentication/identity-adapter.service';
import {User} from '../../../model/authentication/User';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-user-info-page',
  templateUrl: './user-info-page.component.html',
  styleUrls: ['./user-info-page.component.scss']
})
export class UserInfoPageComponent implements OnInit {

  public user: User;
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
    private route: ActivatedRoute,
    private identityAdapter: IdentityAdapterService,
    private router: Router,
    private snackbar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(r => {
      const id = r.get('id');

      this.identityAdapter.getUser(id).subscribe(u => {
        this.user = u;
        u.roles.forEach(rr => {
          this.roles[rr] = true;
        });
      });
    });
  }

  delete(): void {
    const res = confirm('Do you really want to delete user "' + this.user.username + '"?');

    if (res) {
      this.identityAdapter.deleteUser(this.user.id).subscribe(r => {
        if (r) {
          this.router.navigate(['/auth/users']);
        }
        else {
          this.snackbar.open('User deletion failed,', 'X', {duration: 1500});
        }
      }, e => {
        e.error.errors.map(err => err.message).forEach(m => {
          this.snackbar.open(m, 'X', {duration: 1500});
        });
      });
    }
  }
}
