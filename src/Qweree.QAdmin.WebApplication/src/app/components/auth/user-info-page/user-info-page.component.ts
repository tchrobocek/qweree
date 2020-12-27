import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {IdentityAdapterService} from '../../../services/authentication/identity-adapter.service';
import {User} from '../../../model/authentication/User';

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
    private identityAdapter: IdentityAdapterService
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

}
