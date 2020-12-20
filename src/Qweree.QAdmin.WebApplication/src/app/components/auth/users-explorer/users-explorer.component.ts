import { Component, OnInit } from '@angular/core';
import {IdentityAdapterService} from '../../../services/authentication/identity-adapter.service';
import {User} from '../../../model/authentication/User';

@Component({
  selector: 'app-users-explorer',
  templateUrl: './users-explorer.component.html',
  styleUrls: ['./users-explorer.component.scss']
})
export class UsersExplorerComponent implements OnInit {

  public users: User[] = [];

  constructor(
    private identityAdapter: IdentityAdapterService
  ) { }

  ngOnInit(): void {
    this.identityAdapter.getUsers(0, 20, 'Username', 1)
      .subscribe(users => {
        this.users = users;
      });
  }

}
