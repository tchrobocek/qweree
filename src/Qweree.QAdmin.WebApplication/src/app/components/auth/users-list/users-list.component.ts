import {Component, Input, OnInit} from '@angular/core';
import {User} from '../../../model/authentication/User';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
  @Input() public users: User[] = [];

  public orderField = '';
  public orderDir = 0;

  constructor() { }

  ngOnInit(): void {
  }

  // noinspection JSUnusedLocalSymbols
  sortBy(field: string)
  {

  }

}
