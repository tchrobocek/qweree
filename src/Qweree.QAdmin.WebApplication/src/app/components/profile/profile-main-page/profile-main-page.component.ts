import {Component, OnInit} from '@angular/core';
import {TokenStorageService} from '../../../services/authentication/token-storage.service';
import {IdentityAdapterService} from '../../../services/authentication/identity-adapter.service';
import {User} from '../../../model/authentication/User';

@Component({
  selector: 'app-profile-main-page',
  templateUrl: './profile-main-page.component.html',
  styleUrls: ['./profile-main-page.component.scss']
})
export class ProfileMainPageComponent implements OnInit {
  public user: User;

  constructor(
    private tokenStorage: TokenStorageService,
    private identityAdapter: IdentityAdapterService
  ) { }

  ngOnInit(): void {
    this.identityAdapter.getUser(this.tokenStorage.getUserInfo().userId).subscribe(u => {
      this.user = u;
    });
  }

}
